// C# Translation of pytdx/hq.py

using System;
using System.Net.Sockets;

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

        public static Microsoft.Data.Analysis.DataFrame ToDataFrame(IEnumerable<SecurityQuote> quotes)
        {
            var columns = new List<Microsoft.Data.Analysis.DataFrameColumn>
            {
                new Microsoft.Data.Analysis.PrimitiveDataFrameColumn<byte>("Market", quotes.Select(q => q.Market)),
                new Microsoft.Data.Analysis.StringDataFrameColumn("Code", quotes.Select(q => q.Code)),
                new Microsoft.Data.Analysis.PrimitiveDataFrameColumn<double>("Price", quotes.Select(q => q.Price)),
                new Microsoft.Data.Analysis.PrimitiveDataFrameColumn<double>("LastClose", quotes.Select(q => q.LastClose)),
                new Microsoft.Data.Analysis.PrimitiveDataFrameColumn<double>("Open", quotes.Select(q => q.Open)),
                new Microsoft.Data.Analysis.PrimitiveDataFrameColumn<double>("High", quotes.Select(q => q.High)),
                new Microsoft.Data.Analysis.PrimitiveDataFrameColumn<double>("Low", quotes.Select(q => q.Low)),
                new Microsoft.Data.Analysis.PrimitiveDataFrameColumn<int>("Vol", quotes.Select(q => q.Vol)),
                new Microsoft.Data.Analysis.PrimitiveDataFrameColumn<double>("Amount", quotes.Select(q => q.Amount))
                // Add other columns as needed
            };

            return new Microsoft.Data.Analysis.DataFrame(columns);
        }
    }
}
