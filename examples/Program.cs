using System;
using Solana;
using Newtonsoft.Json;

namespace examples
{
    class Program
    {
        static void Main(string[] args)
        {
            //var connection = new Connection("https://devnet.solana.com");
            //var version = connection.GetVersion();
            //Console.WriteLine(JsonConvert.SerializeObject(version));

            var act = new Account(Convert.FromBase64String("9q/laBqLVev2YAX4hcaphE2k+z4H8hY6eqV99LLYq/dwM+KEaZq1hfgAQWQeVWyGdNv0VGWO/a7AcdonWcVlqA=="));
            //8YzYs3Spb41kapwCAkTK9d4HG2w1aq63jZr5WC2U6oyh
            Console.WriteLine(act.PublicKey.ToString());
        }
    }
}
