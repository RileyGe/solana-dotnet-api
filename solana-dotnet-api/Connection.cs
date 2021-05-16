using Nethereum.JsonRpc.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
    }
}
