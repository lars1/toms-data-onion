
#region Copyright Notice
/*
 * RFC3394 Key Wrapping Algorithm
 * Written by Jay Miller
 * 
 * This code is hereby released into the public domain, This applies
 * worldwide.
 */
#endregion

using System;
using System.Diagnostics;

namespace Functions.Crypto
{
   /// <summary>
   /// A <b>Block</b> contains exactly 64 bits of data.  This class
   /// provides several handy block-level operations.
   /// </summary>
   internal class Block
   {
      byte[] _b = new byte[8];

      public Block(Block b) : this(b.Bytes) { }
      public Block(byte[] bytes) : this(bytes, 0) { }
      public Block(byte[] bytes, int index)
      {
         if (bytes == null)
            throw new ArgumentNullException("bytes");
         if (index + 8 > bytes.Length)
            throw new ArgumentException("BufferLengthError", "bytes");
         if (index < 0)
            throw new ArgumentOutOfRangeException("index");

         Array.Copy(bytes, index, _b, 0, 8);
      }

      // Gets the contents of the current Block.
      public byte[] Bytes
      {
         get { return _b; }
      }

      // Returns the contents of the current Block in a hex string.
      public override string ToString()
      {
          //  SoapHexBinary hex = new SoapHexBinary(Bytes);
          //  return hex.ToString();
          return HexFns.ToHex(Bytes);
      }

      // Concatenates the current Block with the specified Block.
      public byte[] Concat(Block right)
      {
         if (right == null)
            throw new ArgumentNullException("right");

         byte[] output = new byte[16];
         
         _b.CopyTo(output, 0);
         right.Bytes.CopyTo(output, 8);

         return output;
      }

      // Converts an array of bytes to an array of Blocks.
      public static Block[] BytesToBlocks(byte[] bytes)
      {
         if (bytes == null)
            throw new ArgumentNullException("bytes");
         if (bytes.Length % 8 != 0)
            throw new ArgumentException("DivisibleBy8Error", "bytes");

         Block[] blocks = new Block[bytes.Length / 8];

         for (int i = 0; i < bytes.Length; i += 8)
            blocks[i / 8] = new Block(bytes, i);

         return blocks;
      }

      // Converts an array of Blocks to an arry of bytes.
      public static byte[] BlocksToBytes(Block[] blocks)
      {
         if (blocks == null)
            throw new ArgumentNullException("blocks");

         byte[] bytes = new byte[blocks.Length * 8];

         for (int i = 0; i < blocks.Length; i++)
            blocks[i].Bytes.CopyTo(bytes, i * 8);

         return bytes;
      }

      // XOR operator against a 64-bit value.
      public static Block operator ^(Block left, long right)
      {
         return Xor(left, right);
      }

      // XORs a block with a 64-bit value.
      public static Block Xor(Block left, long right)
      {
         if (left == null)
            throw new ArgumentNullException("left");

         Block result = new Block(left);
         ReverseBytes(result.Bytes);
         long temp = BitConverter.ToInt64(result.Bytes, 0);

         result = new Block(BitConverter.GetBytes(temp ^ right));
         ReverseBytes(result.Bytes);
         return result;
      }

      // Swaps the byte positions in the specified array.
      internal static void ReverseBytes(byte[] bytes)
      {
         Debug.Assert(bytes != null);
         for (int i = 0; i < bytes.Length / 2; i++)
         {
            byte temp = bytes[i];
            bytes[i] = bytes[(bytes.Length - 1) - i];
            bytes[(bytes.Length - 1) - i] = temp;
         }
      }
   }
}
