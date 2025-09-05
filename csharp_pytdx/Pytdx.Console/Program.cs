using Pytdx;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

Console.WriteLine("Finding best TDX server...");
var bestIp = await TdxHqApi.FindBestIp();

if (bestIp == null)
{
    Console.WriteLine("No available servers found.");
    return;
}

Console.WriteLine($"Best server found: {bestIp.Value.Ip}:{bestIp.Value.Port}. Connecting...");

using (var api = new TdxHqApi(raiseException: true))
{
    if (api.Connect(bestIp.Value.Ip, bestIp.Value.Port))
    {
        Console.WriteLine("Connected successfully.");

        // 1. Get Security Count
        try
        {
            Console.WriteLine("\nFetching security count for Shanghai market (1)...");
            var count = api.GetSecurityCount(1);
            Console.WriteLine($"Number of securities in SH market: {count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching security count: {ex.Message}");
        }

        // 2. Get Security Quotes
        try
        {
            Console.WriteLine("\nFetching quotes for SZ:000001 and SH:600300...");
            var stocks = new List<(byte, string)>
            {
                (0, "000001"), // SZ
                (1, "600300")  // SH
            };
            var quotes = api.GetSecurityQuotes(stocks);

            if (quotes != null)
            {
                foreach (var quote in quotes)
                {
                    Console.WriteLine($"--> Code: {quote.Code}, Price: {quote.Price}, Vol: {quote.Vol}");
                }

                // Example of converting to DataFrame
                var df = TdxHqApi.ToDataFrame(quotes);
                Console.WriteLine("\nDataFrame representation:");
                Console.WriteLine(df);
            }
            else
            {
                Console.WriteLine("Failed to get quotes.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching security quotes: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"--> Inner Exception: {ex.InnerException.Message}");
            }
        }

        Console.WriteLine("\nDisconnecting...");
    }
    else
    {
        Console.WriteLine("Failed to connect.");
    }
}

Console.WriteLine("Disconnected. Program finished.");
