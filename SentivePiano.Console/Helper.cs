using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentivePiano.Console
{
    static class Helper
    {
        public static byte[] AddTimestamp(this byte[] bts)
        {
            return new byte[] { 0x80, 0x80 }.Concat(bts).ToArray();
        }
    }
}
