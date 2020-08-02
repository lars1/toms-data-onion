using System.Text;

namespace Functions
{
    public static class PuzzleFileFns
    {
        public static readonly string StandardOneParamRequiredErrorMessage =
            "\nRequired params:\nparam 1: path to input file\n\nLinux tips: dotnet run -p <project> `realpath <file name>`\nWindows: dotnet run -p <project> .\\<file name>";

        public static int GetLinePayloadStartsAt(string[] allLines) 
        {
            int cursor = 0;
            while (true) 
            {
                if (allLines[cursor].StartsWith("==[ Payload ]==")) 
                {
                    return cursor + 2;
                }
                ++cursor;
            }
        }

        public static string GetPayloadAsOneLongString(string[] allLines, int payloadStartLine) 
        {
            var buffer = new StringBuilder();
            for (var lineNo = payloadStartLine; lineNo < allLines.Length; lineNo++) 
            {
                buffer.Append(allLines[lineNo].Replace("\n", string.Empty).Replace("\r", string.Empty));
            }
            return buffer.ToString();
        }
    }
}