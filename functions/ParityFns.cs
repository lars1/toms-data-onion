namespace Functions
{
    public static class ParityFns
    {
        /// <summary>
        /// True if the byte has an even amount of bits and the parity bit (LSB) is zero,
        /// Or the byte has an odd amount of bits and the parity bit is one.
        /// </summary>
        public static bool OneBitParityCheck(byte rawByte) 
        {
            var parityBit = rawByte & 1;
            var dataBits = (byte)(rawByte & 254); // clears lsb

            var bitCount = BitwiseFns.GetBitCount(dataBits);
            return parityBit == 0 ? bitCount % 2 == 0 : bitCount % 2 == 1;
        }
    }
}