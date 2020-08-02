using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Functions;

namespace layer2
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

            Debug.Assert(payload.StartsWith("<~4J,Yc\"_331o4>"));
            Debug.Assert(payload.EndsWith(@"<Y),nDu&4~>"));
            Debug.Assert(payload.Contains("\n") == false);

            Debug.Assert(ParityFns.OneBitParityCheck(178));
            Debug.Assert(ParityFns.OneBitParityCheck(0b01000001));
            Debug.Assert(ParityFns.OneBitParityCheck(0b00000001) == false);
            Debug.Assert(ParityFns.OneBitParityCheck(0b01001010) == false);

            var bsbTest = new BitStreamBuilder();
            bsbTest.Add7Bits(128);
            bsbTest.Add7Bits(128);
            bsbTest.Add7Bits(128);
            bsbTest.Add7Bits(128);
            bsbTest.Add7Bits(128);
            bsbTest.Add7Bits(128);
            bsbTest.Add7Bits(128);
            bsbTest.Add7Bits(128);
            bsbTest.Add7Bits(128);
            bsbTest.Add7Bits(128);
            var bytesTest = bsbTest.MaterializeStream().ToArray();
            Debug.Assert(bytesTest[0] == 129);
            Debug.Assert(bytesTest[1] == 2);
            Debug.Assert(bytesTest[2] == 4);
            Debug.Assert(bytesTest[3] == 8);
            Debug.Assert(bytesTest[4] == 16);
            Debug.Assert(bytesTest[5] == 32);
            Debug.Assert(bytesTest[6] == 64);
            Debug.Assert(bytesTest[7] == 129);

            var payloadSpan = Ascii85Fns.DecodeAsSpan(payload, 2);
            var builder = new BitStreamBuilder();

            foreach (var aByte in payloadSpan.ToArray())
            {
                if (!ParityFns.OneBitParityCheck(aByte)) continue;
                builder.Add7Bits(aByte);
            }

            var decoded = builder.MaterializeStream().ToArray();
            Console.Write(Encoding.ASCII.GetString(decoded));
            return 0;   
        }
    }
}
