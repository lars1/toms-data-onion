using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Functions.Net
{
  public static class IPv4Fns
    {
        public static int HeaderMinSizeBytes = 20;

        public static IEnumerable<IPv4Frame> IterateFrames(Stream bytes)
        {
            byte[] headerBuffer = new byte[HeaderMinSizeBytes];

            while (bytes.CanRead)
            {
                int bytesRead = bytes.Read(headerBuffer, 0, HeaderMinSizeBytes);

                if (bytesRead != HeaderMinSizeBytes) 
                {
                    yield break;
                }

                int internetHeaderLengthBytes = (int)(headerBuffer[0] & 0xf) * 4;
                if (internetHeaderLengthBytes != HeaderMinSizeBytes) throw new NotImplementedException("Header size other than 20 bytes not supported yet");

                // Calculate header validity
                uint sum = 0;
                for (int ushortOffset = 0; ushortOffset < 20; ushortOffset += 2)
                {
                    sum += ByteFns.MakeUshort(headerBuffer[ushortOffset], headerBuffer[ushortOffset + 1]);
                }
                ushort onesComplementSum = ChecksumFns.AddCarryGetUshort(sum);

                int version = (int)((headerBuffer[0] & 0xf0) >> 4);
                int totalLength = (int) ((headerBuffer[2] << 16) | headerBuffer[3]);
                int protocol = (int)headerBuffer[9];

                byte[] sourceAddressBytes = new byte[4];
                Array.Copy(headerBuffer, 12, sourceAddressBytes, 0, 4);

                byte[] destAddressBytes = new byte[4];
                Array.Copy(headerBuffer, 16, destAddressBytes, 0, 4);

                int optionalBytesToSkip = internetHeaderLengthBytes - HeaderMinSizeBytes; 
                bytes.Position = bytes.Position + optionalBytesToSkip;

                int dataLength = totalLength - internetHeaderLengthBytes;
                byte[] data = new byte[dataLength];

                int dataBytesRead = bytes.Read(data, 0, dataLength);
                if (dataBytesRead != dataLength) throw new InvalidOperationException($"Could not read {dataLength} data bytes, got only {dataBytesRead}");

                yield return new IPv4Frame()
                {
                    Header = new IPv4Header() 
                    {
                        InternetHeaderLength = internetHeaderLengthBytes,
                        TotalLength = totalLength,
                        SourceAddress = new IPAddress(sourceAddressBytes),
                        DestinationAddress = new IPAddress(destAddressBytes),
                        DatagramProtocol = protocol,
                        ChecksumOK = ((ushort)(onesComplementSum ^ 0xffff) == 0),
                        OnesComplementSum = onesComplementSum
                    },
                    Data = data
                };
            }
        }  
    } 
}