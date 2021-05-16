using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NokitaKaze.Base58Check;

namespace Solana
{
    public class PublicKey
    {
        private byte[] bytes;
        public PublicKey()
        {
            bytes = new byte[32];
        }
        public PublicKey(string b58String)
        {
            this.bytes = Base58CheckEncoding.DecodePlain(b58String);
        }
        public PublicKey(byte[] bytes)
        {
            this.bytes = bytes;
        }

        public static PublicKey Default = new PublicKey();

        public override bool Equals(object obj)
        {
            if(obj is PublicKey pk)
            {
                return pk.Bytes.SequenceEqual(bytes);
            }
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return Base58CheckEncoding.EncodePlain(bytes);
        }
        public string ToBase58()
        {
            return ToString();
        }
        public byte[] Bytes
        {
            get
            {
                return bytes;
            }
        }
    }
}
