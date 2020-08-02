using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Functions;

namespace layer1
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

            Debug.Assert(payload.StartsWith(@"<~0/)?#c'P?"));
            Debug.Assert(payload.EndsWith(@"ZOG^Hi`#W@:,!~>"));
            Debug.Assert(payload.Contains("\n") == false);
            Debug.Assert(BitwiseFns.FlipEverySecondBit(180) == 225);
            Debug.Assert(BitwiseFns.RotateByteRight(225) == 240);

            Func<byte, byte> bytePostProcess = 
                (inByte) => BitwiseFns.RotateByteRight(BitwiseFns.FlipEverySecondBit(inByte));

            Debug.Assert(bytePostProcess(180) == 240);

            var payloadSpan = Ascii85Fns.DecodeAsSpan(payload, 2);
            var transformedBytes = payloadSpan.ToArray().Select(bytePostProcess).ToArray();
            Console.Write(Encoding.ASCII.GetString(transformedBytes));

            return 0;          
        }
    }
}
