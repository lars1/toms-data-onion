using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Functions.Net
{
    public static class ChecksumFns
    {
        public static ushort AddCarryGetUshort(uint sum)
        {
            while ((sum & 0xffff0000) != 0)
            {
                sum = (sum & 0xffff) + (sum >> 16);
            }
            return (ushort)sum;
        }


        /// <remarks>
        /// Big endian. Pads last ushort with a zero byte (least significant) if necessary
        /// </remarks> 
        public static IEnumerable<ushort> DataBytesToShorts(byte[] data)
        {
            var oddByteAmount = data.Length % 2 == 1;
            var neatLimit = oddByteAmount ? data.Length - 1 : data.Length;
            for (var i=0; i<neatLimit; i += 2)
            {
                yield return ByteFns.MakeUshort(data[i], data[i+1]);
            }
            if (oddByteAmount) 
            {
                yield return ByteFns.MakeUshort(data[data.Length-1], 0);
            }
            yield break;
        }


        public static uint SumAsUShorts(byte[] data)
        {
            return (uint) data.Select((b, i) => (i & 1) > 0 ? b : (b << 8))
                              .Sum();
        }

        public static ushort FinishChecksum(uint sum)
        {
            return (ushort) (AddCarryGetUshort(sum) ^ 0xffff);
        }
    }
}