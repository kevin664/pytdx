// C# Translation of pytdx/params.py

namespace Pytdx
{
    public static class TdxParams
    {
        // 市场
        public const int MARKET_SZ = 0; // 深圳
        public const int MARKET_SH = 1; // 上海

        // K线种类
        // 0 -   5 分钟K 线
        // 1 -   15 分钟K 线
        // 2 -   30 分钟K 线
        // 3 -   1 小时K 线
        // 4 -   日K 线
        // 5 -   周K 线
        // 6 -   月K 线
        // 7 -   1 分钟
        // 8 -   1 分钟K 线
        // 9 -   日K 线
        // 10 -  季K 线
        // 11 -  年K 线
        public const int KLINE_TYPE_5MIN = 0;
        public const int KLINE_TYPE_15MIN = 1;
        public const int KLINE_TYPE_30MIN = 2;
        public const int KLINE_TYPE_1HOUR = 3;
        public const int KLINE_TYPE_DAILY = 4;
        public const int KLINE_TYPE_WEEKLY = 5;
        public const int KLINE_TYPE_MONTHLY = 6;
        public const int KLINE_TYPE_EXHQ_1MIN = 7;
        public const int KLINE_TYPE_1MIN = 8;
        public const int KLINE_TYPE_RI_K = 9;
        public const int KLINE_TYPE_3MONTH = 10;
        public const int KLINE_TYPE_YEARLY = 11;

        // ref : https://github.com/rainx/pytdx/issues/7
        // 分笔行情最多2000条
        public const int MAX_TRANSACTION_COUNT = 2000;
        // k先数据最多800条
        public const int MAX_KLINE_COUNT = 800;

        // 板块相关参数
        public const string BLOCK_SZ = "block_zs.dat";
        public const string BLOCK_FG = "block_fg.dat";
        public const string BLOCK_GN = "block_gn.dat";
        public const string BLOCK_DEFAULT = "block.dat";
    }
}
