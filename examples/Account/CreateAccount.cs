using System;
using System.Linq;
using Solana;

namespace examples
{
    public class CreateAccount
    {
        public static void Main(string[] args)
        {
            //新建一个随机账号
            var alice = new Account();
            Console.WriteLine(string.Format("Public key of new account is {0}", alice.PublicKey.ToString()));            
            Console.WriteLine(string.Format("Secret key of new account is [{0}]",
                string.Join(",",  Array.ConvertAll(alice.SecretKey, b => Convert.ToUInt32(b)))));

            // 也可以从已知的private key导回account
            // 生成测试账号 => $ solana-keygen new -o ./testaccount.json
            // cat ./testaccount.json 会显示一段u8的array, 把他传到下面变量中
            byte[] pk = new byte[] { 174, 47, 154, 16, 202, 193, 206, 
                113, 199, 190, 53, 133, 169, 175, 31, 56, 222, 53, 138, 
                189, 224, 216, 117, 173, 10, 149, 53, 45, 73, 251, 237, 
                246, 15, 185, 186, 82, 177, 240, 148, 69, 241, 227, 167, 
                80, 141, 89, 240, 121, 121, 35, 172, 247, 68, 251, 226, 
                218, 48, 63, 176, 109, 168, 89, 238, 135 };

            Account act = new Account(pk);
            Console.WriteLine(act.PublicKey.ToString());
            // 此值会等同于 solana address -k ./testaccount.json
        }
    }
}
