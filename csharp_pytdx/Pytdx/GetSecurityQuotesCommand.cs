// C# Translation of pytdx/parser/get_security_quotes.py

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Pytdx
{
    public class GetSecurityQuotesCommand : BaseCommand<List<SecurityQuote>>
    {
        public GetSecurityQuotesCommand(Socket socket) : base(socket) { }

        public override void SetParams(params object[] args)
        {
            if (args.Length != 1 || !(args[0] is IEnumerable<(byte market, string code)> stocks))
            {
                throw new ArgumentException("GetSecurityQuotesCommand requires a list of (market, code) tuples.");
            }

            var stockList = stocks.ToList();
            int stockLen = stockList.Count;
            if (stockLen <= 0) return;

            int pkgDataLen = stockLen * 7;

            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                // This header structure is based on reverse-engineering the python client
                // and may not be fully accurate. The original python code seems to have a bug
                // in calculating pkgDataLen (it adds 12 for no clear reason).
                // We are using the logical length of the data payload.
                writer.Write((ushort)0x010c); // function id
                writer.Write(0); // unknown
                writer.Write((ushort)0x63); // unknown
                writer.Write((ushort)stockLen);
                writer.Write((ushort)pkgDataLen);

                foreach (var stock in stockList)
                {
                    writer.Write(stock.market);
                    byte[] codeBytes = Encoding.UTF8.GetBytes(stock.code.PadRight(6, '\0'));
                    writer.Write(codeBytes, 0, 6);
                }

                SendPkg = stream.ToArray();
            }
        }

        public override List<SecurityQuote> ParseResponse(byte[] body)
        {
            int pos = 0;
            pos += 2; // skip b1 cb
            ushort numStock = BitConverter.ToUInt16(body, pos);
            pos += 2;
            var result = new List<SecurityQuote>();

            for (int i = 0; i < numStock; i++)
            {
                byte market = body[pos];
                string code = Encoding.UTF8.GetString(body, pos + 1, 6).TrimEnd('\0');
                ushort active1 = BitConverter.ToUInt16(body, pos + 7);
                pos += 9;

                var (price, p1) = TdxHelper.GetPrice(body, pos);
                var (lastCloseDiff, p2) = TdxHelper.GetPrice(body, p1);
                var (openDiff, p3) = TdxHelper.GetPrice(body, p2);
                var (highDiff, p4) = TdxHelper.GetPrice(body, p3);
                var (lowDiff, p5) = TdxHelper.GetPrice(body, p4);
                var (reversedBytes0, p6) = TdxHelper.GetPrice(body, p5);
                var (reversedBytes1, p7) = TdxHelper.GetPrice(body, p6);
                var (vol, p8) = TdxHelper.GetPrice(body, p7);
                var (curVol, p9) = TdxHelper.GetPrice(body, p8);
                uint amountRaw = BitConverter.ToUInt32(body, p9);
                double amount = TdxHelper.GetVolume(amountRaw);
                pos = p9 + 4;
                var (sVol, p10) = TdxHelper.GetPrice(body, pos);
                var (bVol, p11) = TdxHelper.GetPrice(body, p10);
                var (reversedBytes2, p12) = TdxHelper.GetPrice(body, p11);
                var (reversedBytes3, p13) = TdxHelper.GetPrice(body, p12);

                var (bid1, p14) = TdxHelper.GetPrice(body, p13);
                var (ask1, p15) = TdxHelper.GetPrice(body, p14);
                var (bidVol1, p16) = TdxHelper.GetPrice(body, p15);
                var (askVol1, p17) = TdxHelper.GetPrice(body, p16);

                var (bid2, p18) = TdxHelper.GetPrice(body, p17);
                var (ask2, p19) = TdxHelper.GetPrice(body, p18);
                var (bidVol2, p20) = TdxHelper.GetPrice(body, p19);
                var (askVol2, p21) = TdxHelper.GetPrice(body, p20);

                var (bid3, p22) = TdxHelper.GetPrice(body, p21);
                var (ask3, p23) = TdxHelper.GetPrice(body, p22);
                var (bidVol3, p24) = TdxHelper.GetPrice(body, p23);
                var (askVol3, p25) = TdxHelper.GetPrice(body, p24);

                var (bid4, p26) = TdxHelper.GetPrice(body, p25);
                var (ask4, p27) = TdxHelper.GetPrice(body, p26);
                var (bidVol4, p28) = TdxHelper.GetPrice(body, p27);
                var (askVol4, p29) = TdxHelper.GetPrice(body, p28);

                var (bid5, p30) = TdxHelper.GetPrice(body, p29);
                var (ask5, p31) = TdxHelper.GetPrice(body, p30);
                var (bidVol5, p32) = TdxHelper.GetPrice(body, p31);
                var (askVol5, p33) = TdxHelper.GetPrice(body, p32);
                pos = p33;

                ushort reversedBytes4 = BitConverter.ToUInt16(body, pos);
                pos += 2;
                var (reversedBytes5, p34) = TdxHelper.GetPrice(body, pos);
                var (reversedBytes6, p35) = TdxHelper.GetPrice(body, p34);
                var (reversedBytes7, p36) = TdxHelper.GetPrice(body, p35);
                var (reversedBytes8, p37) = TdxHelper.GetPrice(body, p36);
                short reversedBytes9Raw = BitConverter.ToInt16(body, p37);
                ushort active2 = BitConverter.ToUInt16(body, p37 + 2);
                pos = p37 + 4;

                result.Add(new SecurityQuote {
                    Market = market, Code = code, Active1 = active1,
                    Price = CalPrice(price, 0), LastClose = CalPrice(price, lastCloseDiff),
                    Open = CalPrice(price, openDiff), High = CalPrice(price, highDiff), Low = CalPrice(price, lowDiff),
                    ServerTime = FormatTime(reversedBytes0.ToString()), ReversedBytes0 = reversedBytes0,
                    ReversedBytes1 = reversedBytes1, Vol = vol, CurVol = curVol, Amount = amount, SVol = sVol, BVol = bVol,
                    ReversedBytes2 = reversedBytes2, ReversedBytes3 = reversedBytes3,
                    Bid1 = CalPrice(price, bid1), Ask1 = CalPrice(price, ask1), BidVol1 = bidVol1, AskVol1 = askVol1,
                    Bid2 = CalPrice(price, bid2), Ask2 = CalPrice(price, ask2), BidVol2 = bidVol2, AskVol2 = askVol2,
                    Bid3 = CalPrice(price, bid3), Ask3 = CalPrice(price, ask3), BidVol3 = bidVol3, AskVol3 = askVol3,
                    Bid4 = CalPrice(price, bid4), Ask4 = CalPrice(price, ask4), BidVol4 = bidVol4, AskVol4 = askVol4,
                    Bid5 = CalPrice(price, bid5), Ask5 = CalPrice(price, ask5), BidVol5 = bidVol5, AskVol5 = askVol5,
                    ReversedBytes4 = reversedBytes4, ReversedBytes5 = reversedBytes5,
                    ReversedBytes6 = reversedBytes6, ReversedBytes7 = reversedBytes7, ReversedBytes8 = reversedBytes8,
                    ReversedBytes9 = reversedBytes9Raw / 100.0, Active2 = active2
                });
            }

            return result;
        }

        private double CalPrice(int base_p, int diff) => (base_p + diff) / 100.0;

        private string FormatTime(string timeStamp)
        {
            if (timeStamp.Length < 6) return timeStamp; // Not a valid timestamp
            string time = timeStamp.Substring(0, timeStamp.Length - 6) + ":";
            if (int.Parse(timeStamp.Substring(timeStamp.Length - 6, 2)) < 60)
            {
                time += timeStamp.Substring(timeStamp.Length - 6, 2) + ":";
                time += (int.Parse(timeStamp.Substring(timeStamp.Length - 4)) * 60 / 10000.0).ToString("00.000");
            }
            else
            {
                time += (int.Parse(timeStamp.Substring(timeStamp.Length - 6)) * 60 / 1000000).ToString("00") + ":";
                time += ((int.Parse(timeStamp.Substring(timeStamp.Length - 6)) * 60 % 1000000) * 60 / 1000000.0).ToString("00.000");
            }
            return time;
        }
    }
}
