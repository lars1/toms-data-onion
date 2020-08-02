using System;
using System.Collections.Generic;

namespace Functions
{
    public static class BitwiseFns 
    {
        private static readonly byte highBitMask = 128;

        public static byte RotateByteRight(byte input)
        {
            byte rotated = (byte) (input >> 1);

            if ((input & 1) > 0) 
            {
                rotated ^= highBitMask;  // bit rotated from low to highest bit
            }
            return rotated;
        }

        private static readonly byte flipBitsMask = 0b01010101;

        public static byte FlipEverySecondBit(byte input)
        {
            return (byte) (input ^ flipBitsMask);
        }

        private static Dictionary<byte, int> NibbleBitAmount = new Dictionary<byte, int>
        {
            [0] = 0,    // 0000
            [1] = 1,    // 0001
            [2] = 1,    // 0010
            [3] = 2,    // 0011
            [4] = 1,    // 0100
            [5] = 2,    // 0101
            [6] = 2,    // 0110
            [7] = 3,    // 0111
            [8] = 1,    // 1000
            [9] = 2,    // 1001
            [10] = 2,   // 1010
            [11] = 3,   // 1011
            [12] = 2,   // 1100
            [13] = 3,   // 1101
            [14] = 3,   // 1110
            [15] = 4    // 1111
        };

        public static int GetBitCount(byte input) 
        {
            return NibbleBitAmount[(byte)(input >> 4)] + NibbleBitAmount[(byte)(input & 15)];
        }

        public static byte ReverseBits(byte input)
        {
            byte outputByte = 0;
            for (int i=0; i<8; i++) 
            {
                outputByte = (byte)(outputByte << 1 | input & 1);
                input = (byte)(input >> 1);
            }
            return outputByte;
        }
    }
}