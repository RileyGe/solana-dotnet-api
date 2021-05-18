using Nethereum.JsonRpc.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Solana
{
    public class Connection
    {
        private RpcClient client;
        public Connection(string url = @"http://localhost:8899") 
        {
            client = new RpcClient(new Uri(url));
        }
        public Version GetVersion()
        {            
            var result = GetVersionAsync();
            result.Wait();
            return result.Result;
        }
        public Task<Version> GetVersionAsync()
        {
            return client.SendRequestAsync<Version>("getVersion");
        }

        public string RequestAirdrop(PublicKey to, ulong amount, string commitment = "finalized")
        {
            var result = RequestAirdropAsync(to, amount, commitment);
            result.Wait();
            return result.Result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public Task<string> RequestAirdropAsync(PublicKey to, ulong amount, string commitment = "finalized")
        {
            var cmtDict = new Dictionary<string, string>
            {
                { "commitment", commitment }
            };
            return client.SendRequestAsync<string>("requestAirdrop", null, 
                new object[] { to.ToBase58(), amount, cmtDict });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="commitment"></param>
        /// <returns></returns>
        public ulong GetBalance(PublicKey address, string commitment = "finalized")
        {
            var result = GetBalanceAsync(address, commitment);
            result.Wait();
            return result.Result.Value<ulong>("value");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="commitment"></param>
        /// <returns></returns>
        public Task<JObject> GetBalanceAsync(PublicKey address, string commitment = "finalized")
        {
            var cmtDict = new Dictionary<string, string>
            {
                { "commitment", commitment }
            };
            return client.SendRequestAsync<JObject>("getBalance", null,
                new object[] { address.ToBase58(), cmtDict });
        }
    }
}
