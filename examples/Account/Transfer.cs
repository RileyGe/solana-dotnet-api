using Solana;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace examples
{
    public class Transfer
    {
        public static void Main(string[] args)
        {
            // 可以在本地用 solana-test-validator 起一個測試的鏈方便測試
            const string url = "https://api.devnet.solana.com";
            var connection = new Connection(url);
            // 這個account是預產的 可以透過前面學習到的步驟產完然後帶進來
            // prettier-ignore
            var feePayer = new Account(new byte[] { 206, 54, 90, 62, 42, 169, 79, 30, 10, 214, 71, 
                58, 161, 79, 210, 133, 123, 207, 196, 142, 168, 155, 129, 108, 35, 155, 218, 75, 82, 
                233, 79, 40, 67, 120, 93, 30, 66, 81, 199, 231, 199, 75, 70, 229, 64, 75, 252, 105, 
                43, 152, 135, 212, 92, 179, 44, 129, 174, 181, 26, 186, 90, 20, 83, 69 });

            var to = new Account(new byte[] { 174, 47, 154, 16, 202, 193, 206,
                113, 199, 190, 53, 133, 169, 175, 31, 56, 222, 53, 138,
                189, 224, 216, 117, 173, 10, 149, 53, 45, 73, 251, 237,
                246, 15, 185, 186, 82, 177, 240, 148, 69, 241, 227, 167,
                80, 141, 89, 240, 121, 121, 35, 172, 247, 68, 251, 226,
                218, 48, 63, 176, 109, 168, 89, 238, 135 });

            var programId = new PublicKey("11111111111111111111111111111111");
            var insIndex = 2;
            ulong amount = 1000000000;
            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);
            writer.Write(insIndex);
            writer.Write(amount);
            var transferData = ms.ToArray();
        }
    }
}
