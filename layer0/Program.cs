using System;
using System.IO;
using System.Diagnostics;
using Functions;
using System.Text;

namespace layer0
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 1) 
            {
                Console.WriteLine("\nRequired params:\nparam 1: path to original TDO file");
                Console.WriteLine("\nLinux tips: dotnet run -p layer0 `realpath toms-data-onion.txt`");
                return 1;
            }

            var allLines = File.ReadAllLines(args[0]);
            int payloadStartLine = PuzzleFileFns.GetLinePayloadStartsAt(allLines);
            var payload = PuzzleFileFns.GetPayloadAsOneLongString(allLines, payloadStartLine);

            Debug.Assert(payload.StartsWith(@"<~4[!!l9OW3XEZ"));
            Debug.Assert(payload.EndsWith(@"\d<,?I,k/0TI+$46~>"));
            Debug.Assert(payload.Contains("\n") == false);

            var payloadSpan = Ascii85Fns.DecodeAsSpan(payload, 2);
            Console.Write(Encoding.ASCII.GetString(payloadSpan.ToArray()));

            return 0;
        }
    }
}
