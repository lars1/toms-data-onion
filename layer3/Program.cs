using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using Functions;
using System.Text;

namespace layer3
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 1) 
            {
                Console.WriteLine(PuzzleFileFns.StandardOneParamRequiredErrorMessage);
                return 1;
            }

            var allLines = File.ReadAllLines(args[0]);
            int payloadStartLine = PuzzleFileFns.GetLinePayloadStartsAt(allLines);
            var payload = PuzzleFileFns.GetPayloadAsOneLongString(allLines, payloadStartLine);

            Debug.Assert(payload.StartsWith("<~;&S8o%X?BS"));
            Debug.Assert(payload.EndsWith(@"uQ&0qa8h%~>"));
            Debug.Assert(payload.Contains("\n") == false);

            var cipherBytes = Ascii85Fns.DecodeAsSpan(payload, 2).ToArray();

            // We know what the plaintext should start with, so those bytes are easy to find:
            var plainText1Bytes = Encoding.ASCII.GetBytes("==[ Layer 4/5: ");
            var first15KeyBytes = ByteArrayFns.XOR(cipherBytes.Take(15).ToArray(), plainText1Bytes).ToArray();
           
            var partialOneTimePad = first15KeyBytes.Concat(new byte[17]).ToArray();

            // We also know the line "==[ Payload ]===============================================" should appear, and it's position is easy to find from the partially descrambled file:
            var partialDescramble = Encoding.ASCII.GetString(ByteArrayFns.OneTimePadScramble(cipherBytes, partialOneTimePad).ToArray());
                       

            var offsetToUnscrambled = partialDescramble.IndexOf("==[ Pay") + 7;
            var stillScrambledBytes = cipherBytes.Skip(offsetToUnscrambled).Take(17).ToArray();
            
            var expectedPlainTextBytes = Encoding.ASCII.GetBytes("load ]===========");

            var last17KeyBytes = ByteArrayFns.XOR(stillScrambledBytes, expectedPlainTextBytes).ToArray();
            
            var finalOneTimePad = first15KeyBytes.Concat(last17KeyBytes).ToArray();
            var plainTextBytes = ByteArrayFns.OneTimePadScramble(cipherBytes, finalOneTimePad).ToArray();
            Console.WriteLine(Encoding.ASCII.GetString(plainTextBytes));
            return 0;
        }
    }
}
