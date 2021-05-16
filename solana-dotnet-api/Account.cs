using System;
using System.Linq;
using Org.BouncyCastle.Security;

namespace Solana
{
    public class Account
    {
        //private Nacl.SignKeyPair pair;
        private byte[] secretKey;
        private byte[] publicKey;
        public Account(byte[] secretKey = null)
        {
            if(secretKey is null)
            {
                secretKey = new byte[64];
                (new SecureRandom()).NextBytes(secretKey);
                //Nacl.TweetNaCl.RandomBytes(secretKey);
            }
            if(secretKey.Length != 64)
            {
                throw new ArgumentException("bad secret key size");
            }
            publicKey = secretKey.Skip(32).ToArray();
            this.secretKey = secretKey;            
        }
        public PublicKey PublicKey
        {
            get
            {
                return new PublicKey(publicKey);
            }
        }
        public byte[] SecretKey
        {
            get
            {
                return secretKey;
            }
        }
    }
}
