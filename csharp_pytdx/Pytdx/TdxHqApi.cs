// C# Translation of pytdx/hq.py

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Data.Analysis;


namespace Pytdx
{
    public class TdxHqApi : BaseSocketClient
    {
        public TdxHqApi(bool multithread = false, bool heartbeat = false, bool autoRetry = false, bool raiseException = false)
            : base(multithread, heartbeat, autoRetry, raiseException)
        {
        }

        protected override void Setup()
        {
            // This is the handshake process after connecting
            var cmd1 = new SetupCommand1(_socket);
            cmd1.SetParams();
            cmd1.CallApi();

            var cmd2 = new SetupCommand2(_socket);
            cmd2.SetParams();
            cmd2.CallApi();

            var cmd3 = new SetupCommand3(_socket);
            cmd3.SetParams();
            cmd3.CallApi();
        }

        /// <summary>
        /// Gets the number of securities in a given market.
        /// </summary>
        /// <param name="market">The market ID (0 for SZ, 1 for SH).</param>
        /// <returns>The number of securities.</returns>
        public int GetSecurityCount(ushort market)
        {
            return ApiRequest(socket =>
            {
                var command = new GetSecurityCountCommand(socket);
                command.SetParams(market);
                return command.CallApi();
            });
        }

        public List<SecurityQuote> GetSecurityQuotes(List<(byte market, string code)> stocks)
        {
            return ApiRequest(socket =>
            {
                var command = new GetSecurityQuotesCommand(socket);
                command.SetParams(stocks);
                return command.CallApi();
            });
        }

        // Other API methods will be added here...

        public static DataFrame ToDataFrame(IEnumerable<SecurityQuote> quotes)
        {
            var columns = new List<DataFrameColumn>
            {
                new PrimitiveDataFrameColumn<byte>("Market", quotes.Select(q => q.Market)),
                new StringDataFrameColumn("Code", quotes.Select(q => q.Code)),
                new PrimitiveDataFrameColumn<double>("Price", quotes.Select(q => q.Price)),
                new PrimitiveDataFrameColumn<double>("LastClose", quotes.Select(q => q.LastClose)),
                new PrimitiveDataFrameColumn<double>("Open", quotes.Select(q => q.Open)),
                new PrimitiveDataFrameColumn<double>("High", quotes.Select(q => q.High)),
                new PrimitiveDataFrameColumn<double>("Low", quotes.Select(q => q.Low)),
                new PrimitiveDataFrameColumn<int>("Vol", quotes.Select(q => q.Vol)),
                new PrimitiveDataFrameColumn<double>("Amount", quotes.Select(q => q.Amount))
                // Add other columns as needed
            };

            return new DataFrame(columns);
        }

        public static async Task<(string Ip, int Port)?> FindBestIp(int top = 5)
        {
            var tasks = HqHosts.List.Select(Ping).ToList();
            var results = await Task.WhenAll(tasks);

            var bestHost = results
                .Where(r => r.Time.HasValue)
                .OrderBy(r => r.Time.Value)
                .FirstOrDefault();

            return bestHost.Host;
        }

        private static async Task<((string Ip, int Port)? Host, TimeSpan? Time)> Ping((string Name, string Ip, int Port) hostInfo)
        {
            var api = new TdxHqApi();
            try
            {
                var stopwatch = Stopwatch.StartNew();
                bool success = false;
                await Task.Run(() =>
                {
                    if (api.Connect(hostInfo.Ip, hostInfo.Port, timeout: 2000))
                    {
                        var count = api.GetSecurityCount(0); // Market 0 for SZ
                        if (count > 1000)
                        {
                            success = true;
                        }
                    }
                });
                stopwatch.Stop();

                if (success)
                {
                    Console.WriteLine($"Ping {hostInfo.Name} ({hostInfo.Ip}:{hostInfo.Port}) - {stopwatch.ElapsedMilliseconds} ms - OK");
                    return ((hostInfo.Ip, hostInfo.Port), stopwatch.Elapsed);
                }
                else
                {
                    Console.WriteLine($"Ping {hostInfo.Name} ({hostInfo.Ip}:{hostInfo.Port}) - FAILED (No valid response)");
                    return (null, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ping {hostInfo.Name} ({hostInfo.Ip}:{hostInfo.Port}) - FAILED ({ex.GetType().Name})");
                return (null, null);
            }
            finally
            {
                api.Disconnect();
            }
        }
    }
}
