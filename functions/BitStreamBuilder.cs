using System;
using System.IO;

namespace Functions
{
    public class BitStreamBuilder
    {
        private MemoryStream memoryStream;
        private byte bitAccumulator;
        private int bitsAvailable = 8;
        private bool streamMaterialized = false;

        public BitStreamBuilder()
        {
            memoryStream = new MemoryStream();
        }


        /// <remarks>
        /// Adds seven bits at at time to to a buffer, gradually building up a set of valid bytes (8 bits of course).
        /// I thought the bytes were to be built by first filling up the least significant bits, then the most significant one(s).
        /// But that is completely wrong.
        /// Instead the output bytes are built up by adding most significant bits first, then filling in the least signifcant one(s).
        /// </remarks>
        public void Add7Bits(byte b)
        {
            if (streamMaterialized) throw new InvalidOperationException("Stream already materialized");

            var bitsOverflowing = bitsAvailable == 8 ? 0 : 7 - bitsAvailable;

            if (bitsAvailable == 8) {
                bitAccumulator = (byte)(b & 0b11111110);
                bitsAvailable = 1;
            }
            else 
            {
                bitAccumulator |= (byte)(b >> (8 - bitsAvailable));
                bitsAvailable += 1;
            }
            
            if (bitsAvailable != 1) 
            {
                memoryStream.WriteByte(bitAccumulator);
                bitAccumulator = 0;
            }

            var bitsNoParity = (byte)(b & 254);

            if (0 < bitsOverflowing && bitsOverflowing < 7)
            {
                bitAccumulator = (byte)(bitsNoParity << (7 - bitsOverflowing));
            }
        }

        public MemoryStream MaterializeStream() 
        {
            if (bitsAvailable != 8)
            {
                memoryStream.WriteByte(bitAccumulator);
            }
            streamMaterialized = true;
            memoryStream.Position = 0L;
            return memoryStream;
        }
    } 
}