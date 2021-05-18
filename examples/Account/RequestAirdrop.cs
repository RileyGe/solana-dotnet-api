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

            var alice = new Account();
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
