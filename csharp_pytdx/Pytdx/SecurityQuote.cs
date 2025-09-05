// C# Data Structure for the result of GetSecurityQuotes

namespace Pytdx
{
    public record SecurityQuote
    {
        public byte Market { get; init; }
        public string Code { get; init; }
        public int Active1 { get; init; }
        public double Price { get; init; }
        public double LastClose { get; init; }
        public double Open { get; init; }
        public double High { get; init; }
        public double Low { get; init; }
        public string ServerTime { get; init; }
        public int ReversedBytes0 { get; init; }
        public int ReversedBytes1 { get; init; }
        public int Vol { get; init; } //成交量
        public int CurVol { get; init; }
        public double Amount { get; init; }
        public int SVol { get; init; } //内盘
        public int BVol { get; init; } //外盘
        public int ReversedBytes2 { get; init; }
        public int ReversedBytes3 { get; init; }
        public double Bid1 { get; init; }
        public double Ask1 { get; init; }
        public int BidVol1 { get; init; }
        public int AskVol1 { get; init; }
        public double Bid2 { get; init; }
        public double Ask2 { get; init; }
        public int BidVol2 { get; init; }
        public int AskVol2 { get; init; }
        public double Bid3 { get; init; }
        public double Ask3 { get; init; }
        public int BidVol3 { get; init; }
        public int AskVol3 { get; init; }
        public double Bid4 { get; init; }
        public double Ask4 { get; init; }
        public int BidVol4 { get; init; }
        public int AskVol4 { get; init; }
        public double Bid5 { get; init; }
        public double Ask5 { get; init; }
        public int BidVol5 { get; init; }
        public int AskVol5 { get; init; }
        public int ReversedBytes4 { get; init; }
        public int ReversedBytes5 { get; init; }
        public int ReversedBytes6 { get; init; }
        public int ReversedBytes7 { get; init; }
        public int ReversedBytes8 { get; init; }
        public double ReversedBytes9 { get; init; } // 涨速
        public int Active2 { get; init; }
    }
}
