using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Functions
{
    public static class Ascii85Fns 
    { 
        private static UInt32[] decodeCoeffs = {
            Convert.ToUInt32(Math.Pow(85, 4)), 
            Convert.ToUInt32(Math.Pow(85, 3)),
            Convert.ToUInt32(Math.Pow(85, 2)),
            Convert.ToUInt32(85),
            1
        };

        public static ReadOnlySpan<byte> DecodeAsSpan(string ascii85String, int baseOffset)
        {
            var offset = baseOffset;
            var buffer = new MemoryStream(ascii85String.Length / 5 * 4);
            
            while (offset <= ascii85String.Length - 7)
            {
                if (ascii85String[offset] == 'z') {
                    throw new NotImplementedException("All zero group encountered");
                }
                var bytes = DecodeGroupAsBytes(ascii85String.Substring(offset, 5));
                buffer.Write(bytes, 0, bytes.Length);
                offset += 5;
            }
            
            if (offset < ascii85String.Length - 2) 
            {
                // padding with 1 to 4 bytes:
                var lastGroup = ascii85String.Substring(offset, ascii85String.Length - (offset + 2));
                var paddedLastGroup = lastGroup + "uuuuu".Substring(lastGroup.Length);

                var decodedBytes = DecodeGroupAsBytes(paddedLastGroup);
                var bytes = (new List<byte>(decodedBytes)).GetRange(0, lastGroup.Length - 1).ToArray();
                buffer.Write(bytes, 0, bytes.Length);
            }
            return (ReadOnlySpan<byte>) buffer.ToArray();
        }


        private static byte[] DecodeGroupAsBytes(string fiveLetterGroup)
        {
            Debug.Assert(fiveLetterGroup.Length == 5);

            // Convert chars to raw number values, minus 33:
            var digits = new UInt32[]
            {
                Convert.ToUInt32(fiveLetterGroup[0]) - 33,
                Convert.ToUInt32(fiveLetterGroup[1]) - 33,
                Convert.ToUInt32(fiveLetterGroup[2]) - 33,
                Convert.ToUInt32(fiveLetterGroup[3]) - 33,
                Convert.ToUInt32(fiveLetterGroup[4]) - 33
            };

            // Make original bit pattern:
            UInt32 fourBytes =
                digits[0] * decodeCoeffs[0] +
                digits[1] * decodeCoeffs[1] +
                digits[2] * decodeCoeffs[2] +
                digits[3] * decodeCoeffs[3] +
                digits[4] * decodeCoeffs[4];

            // Interpret the bit pattern as four ascii chars:
            byte[] asciiBytes = new byte[] {
                Convert.ToByte((fourBytes >> 24) & 255),
                Convert.ToByte((fourBytes >> 16) & 255),
                Convert.ToByte((fourBytes >> 8) & 255),
                Convert.ToByte((fourBytes) & 255),
            };

            return asciiBytes;
        }
    }
}