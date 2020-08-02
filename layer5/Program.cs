using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Functions;
using Functions.Crypto;

namespace layer5
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

            Debug.Assert(payload.StartsWith("<~$O=cdK]PkWZ"));
            Debug.Assert(payload.EndsWith("/UHOHO4@~>"));
            Debug.Assert(payload.Contains("\n") == false);

            var payloadSpan = Ascii85Fns.DecodeAsSpan(payload, 2);
            
            var kekSpan = payloadSpan.Slice(0, 32);
            var ivWrapSpan = payloadSpan.Slice(32, 8);
            var encryptedKeySpan = payloadSpan.Slice(40, 40);
            var ivSpan = payloadSpan.Slice(80, 16);
            var encryptedPayloadSpan = payloadSpan.Slice(96);

            Debug.Assert(HexFns.ToHex(kekSpan.ToArray()) == "0b07c91284e54bd5b3a356451eda9d38e9770f505fef14f0e62c74f3fd1f286a");
            Debug.Assert(HexFns.ToHex(ivWrapSpan.ToArray()) == "badc0ffeebadc0de");
            Debug.Assert(HexFns.ToHex(ivSpan.ToArray()) == "8c2e487274dbebe699677f1b54ebbeb4");

            var plainTextKeyBytes = CryptoFns.DecryptKey(kekSpan.ToArray(), 
                                        ivWrapSpan.ToArray(), encryptedKeySpan.ToArray());

            Debug.Assert(HexFns.ToHex(plainTextKeyBytes) == "c68cade3c9c973067b383b77713ddac8072e05d2bb54a8a2d45b312ce8817024");

            var plainText = CryptoFns.Decrypt(plainTextKeyBytes, ivSpan.ToArray(), 
                                        encryptedPayloadSpan.ToArray());

            Console.WriteLine(Encoding.ASCII.GetString(plainText));
            return 0;
        }
    }
}
