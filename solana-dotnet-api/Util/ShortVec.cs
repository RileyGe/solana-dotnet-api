using System;
using System.Collections.Generic;
using System.Text;

namespace Solana.Util
{
    public class ShortVec
    {
        public static ulong decodeLength(byte[] bytes) {
            ulong len = 0;
            ulong size = 0;
            //while (true) {
            //    let elem = bytes.shift() as number;
            //    len |= (elem & 0x7f) << (size * 7);
            //    size += 1;
            //    if ((elem & 0x80) == 0) {
            //        break;
            //    }
            //}
            return len;
        }

        public static byte[] encodeLength(int len) {
            var rem_len = len;
            List<byte> bytes = new List<byte>();
            while (true)
            {
                var elem = rem_len & 0x7f;
                rem_len >>= 7;
                if (rem_len == 0)
                {
                    bytes.Add(Convert.ToByte(elem));
                    break;
                }
                else
                {
                    elem |= 0x80;
                    bytes.Add(Convert.ToByte(elem));
                }
            }
            return bytes.ToArray();
        }
    }
}
