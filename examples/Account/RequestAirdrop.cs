using Newtonsoft.Json;
using Solana;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace examples
{
    public class RequestAirdrop
    {
        public static void Main(string[] args)
        {
            //使用devnet的api
            var connection = new Connection("https://api.devnet.solana.com");
            //获取节点的一些基本信息
            var version = connection.GetVersion();
            Console.WriteLine(JsonConvert.SerializeObject(version));

            var alice = new Account(new byte[] { 206, 54, 90, 62, 42, 169, 79, 30, 10, 214, 71,
                58, 161, 79, 210, 133, 123, 207, 196, 142, 168, 155, 129, 108, 35, 155, 218, 75, 82,
                233, 79, 40, 67, 120, 93, 30, 66, 81, 199, 231, 199, 75, 70, 229, 64, 75, 252, 105,
                43, 152, 135, 212, 92, 179, 44, 129, 174, 181, 26, 186, 90, 20, 83, 69 });
            Console.WriteLine(alice.PublicKey.ToBase58());
            var hashid = connection.RequestAirdrop(alice.PublicKey, Convert.ToUInt64(1e10));
            Console.WriteLine(hashid);
            while (true)
            {
                var balance = connection.GetBalance(alice.PublicKey);
                if (balance > 0)
                {
                    Console.WriteLine("Balance:" + balance.ToString());
                    break;
                }
                else
                {
                    Thread.Sleep(5 * 1000);
                }
            }
        }
    }
}
