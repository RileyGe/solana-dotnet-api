using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Solana
{
    public class Account
    {
        private Nacl.SignKeyPair pair;
        public Account(byte[] secretKey = null)
        {
            if(secretKey is null)
            {
                secretKey = new byte[64];
                Nacl.TweetNaCl.RandomBytes(secretKey);
            }
            if(secretKey.Length != 64)
            {
                throw new ArgumentException("bad secret key size");
            }
            var pk = secretKey.Skip(32).ToArray();
            pair = new Nacl.SignKeyPair(pk, secretKey);            
        }
        public PublicKey PublicKey
        {
            get
            {
                return new PublicKey(pair.PublicKey);
            }
        }
        public byte[] SecretKey
        {
            get
            {
                return pair.SecretKey;
            }
        }
    }
}
