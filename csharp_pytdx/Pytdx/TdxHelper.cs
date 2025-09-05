// C# Translation of pytdx/helper.py

using System;
using System.IO;
using System.Text;

namespace Pytdx
{
    public static class TdxHelper
    {
        /// <summary>
        /// XXX: 分析了一下，貌似是类似utf-8的编码方式保存有符号数字
        /// </summary>
        public static (int, int) GetPrice(byte[] data, int pos)
        {
            int posByte = 6;
            byte bdata = data[pos];
            int intdata = bdata & 0x3f;
            bool sign = (bdata & 0x40) != 0;

            if ((bdata & 0x80) != 0)
            {
                while (true)
                {
                    pos += 1;
                    bdata = data[pos];
                    intdata += (bdata & 0x7f) << posByte;
                    posByte += 7;

                    if ((bdata & 0x80) == 0)
                    {
                        break;
                    }
                }
            }

            pos += 1;

            if (sign)
            {
                intdata = -intdata;
            }

            return (intdata, pos);
        }

        public static double GetVolume(uint ivol)
        {
            int logpoint = (int)(ivol >> 24);
            int hleax = (int)((ivol >> 16) & 0xff);
            int lheax = (int)((ivol >> 8) & 0xff);
            int lleax = (int)(ivol & 0xff);

            int dwEcx = logpoint * 2 - 0x7f;
            int dwEdx = logpoint * 2 - 0x86;
            int dwEsi = logpoint * 2 - 0x8e;
            int dwEax = logpoint * 2 - 0x96;

            double dbl_xmm6 = Math.Pow(2.0, Math.Abs(dwEcx));
            if (dwEcx < 0)
            {
                dbl_xmm6 = 1.0 / dbl_xmm6;
            }

            double dbl_xmm4;
            if ((hleax & 0x80) != 0)
            {
                int dwtmpeax = dwEdx + 1;
                double tmpdbl_xmm3 = Math.Pow(2.0, dwtmpeax);
                double dbl_xmm0 = Math.Pow(2.0, dwEdx) * 128.0;
                dbl_xmm0 += (hleax & 0x7f) * tmpdbl_xmm3;
                dbl_xmm4 = dbl_xmm0;
            }
            else
            {
                double dbl_xmm0;
                if (dwEdx >= 0)
                {
                    dbl_xmm0 = Math.Pow(2.0, dwEdx) * hleax;
                }
                else
                {
                    // In C#, Pow with negative exponent is 1/Pow(base, -exp)
                    dbl_xmm0 = (1 / Math.Pow(2.0, -dwEdx)) * hleax;
                }
                dbl_xmm4 = dbl_xmm0;
            }

            double dbl_xmm3 = Math.Pow(2.0, dwEsi) * lheax;
            double dbl_xmm1 = Math.Pow(2.0, dwEax) * lleax;

            if ((hleax & 0x80) != 0)
            {
                dbl_xmm3 *= 2.0;
                dbl_xmm1 *= 2.0;
            }

            return dbl_xmm6 + dbl_xmm4 + dbl_xmm3 + dbl_xmm1;
        }

        public static (int, int, int, int, int, int) GetDateTime(int category, byte[] buffer, int pos)
        {
            int year, month, day, hour = 15, minute = 0;

            if (category < 4 || category == 7 || category == 8)
            {
                ushort zipday = BitConverter.ToUInt16(buffer, pos);
                ushort tminutes = BitConverter.ToUInt16(buffer, pos + 2);

                year = (zipday >> 11) + 2004;
                month = (zipday % 2048) / 100;
                day = (zipday % 2048) % 100;
                hour = tminutes / 60;
                minute = tminutes % 60;
            }
            else
            {
                uint zipday = BitConverter.ToUInt32(buffer, pos);
                year = (int)(zipday / 10000);
                month = (int)((zipday % 10000) / 100);
                day = (int)(zipday % 100);
            }

            pos += 4;
            return (year, month, day, hour, minute, pos);
        }

        public static (int, int, int) GetTime(byte[] buffer, int pos)
        {
            ushort tminutes = BitConverter.ToUInt16(buffer, pos);
            int hour = tminutes / 60;
            int minute = tminutes % 60;
            pos += 2;
            return (hour, minute, pos);
        }
    }
}
