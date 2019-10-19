using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SentivePiano
{
    public static class Helper
    {
        public static string PrintInHex(this byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var b in bytes)
            {
                var s = Convert.ToString(b, 16);
                if (s.Length == 1)
                {
                    s = "0" + s;
                }

                sb.Append(s);
            }

            return sb.ToString();
        }
        public static byte[] AddTimestamp(this byte[] bts)
        {
            return new byte[] { 0x80, 0x80 }.Concat(bts).ToArray();
        }

        public static byte[] HexStringToBytes(this string hexString)
        {
            hexString = hexString.Replace(" ", "").Replace("-", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
    }
}