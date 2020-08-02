using System;
using System.Collections.Generic;

namespace Functions
{
    public static class ByteArrayFns 
    {
        public static IEnumerable<byte> XOR(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) throw new ArgumentException("Uneven array lengths");
            for (int i=0; i<a.Length; i++)
            {
                yield return (byte) (a[i] ^ b[i]);
            }
        }

        public static IEnumerable<byte> OneTimePadScramble(byte[] cipherText, byte[] oneTimePad)
        {
            int offset = 0; 
            foreach (var cByte in cipherText)
            {
                yield return (byte) (cByte ^ oneTimePad[offset % oneTimePad.Length]);
                ++offset;
            }
        }
    }
}