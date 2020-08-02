using System.Linq;

namespace Functions
{
    public static class HexFns
    {
        public static string ToHex(byte[] data)
        {
            return string.Join(string.Empty, data.Select(b => $"{b:x2}"));
        } 
    }
}