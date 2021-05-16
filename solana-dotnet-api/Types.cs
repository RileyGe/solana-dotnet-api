using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solana
{
    [JsonObject]
    public class Version
    {
        //{ 'feature-set': 3714435735, 'solana-core': '1.6.6' }
        [JsonProperty(PropertyName = "feature-set")]
        public long FeatureSet;
        [JsonProperty(PropertyName = "solana-core")]
        public string SolanaCore;
    }
}
