using System;
using System.Linq;
using System.Net;
using System.Text;

namespace Functions.Net
{
  public static class UDPFns
    {
        public static int UDPHeaderLength = 8;

        public static UDPFrame DecodeUDP(byte[] datagramBytes)
        {
            if (datagramBytes.Length < UDPHeaderLength) throw new ArgumentException(nameof(UDPHeaderLength));

            ushort sourcePort = ByteFns.MakeUshort(datagramBytes[0], datagramBytes[1]);
            ushort destPort = ByteFns.MakeUshort(datagramBytes[2], datagramBytes[3]);
            ushort headerAndDataLength = ByteFns.MakeUshort(datagramBytes[4], datagramBytes[5]);
            ushort checksum = ByteFns.MakeUshort(datagramBytes[6], datagramBytes[7]);
            var dataBuffer = new byte[datagramBytes.Length - UDPHeaderLength];
            Array.Copy(datagramBytes, UDPHeaderLength, dataBuffer, 0, datagramBytes.Length - UDPHeaderLength);

            return new UDPFrame
            {
                Header = new UDPHeader() 
                {
                    SourcePort = sourcePort,
                    DestinationPort = destPort,
                    HeaderAndDataLength = headerAndDataLength,
                    Checksum = checksum
                },
                Data = dataBuffer
            };
        }

        public static ushort UDPChecksum(IPAddress sourceAddress, IPAddress destinationAddress, UDPFrame packet)
        {
            uint checksum = 0;
            
            checksum += ChecksumFns.SumAsUShorts(sourceAddress.GetAddressBytes());
            checksum += ChecksumFns.SumAsUShorts(destinationAddress.GetAddressBytes());
            checksum += 17;   // protocol udp = 17
            checksum += packet.Header.HeaderAndDataLength;
            checksum += packet.Header.SourcePort;
            checksum += packet.Header.DestinationPort;
            checksum += packet.Header.HeaderAndDataLength;       
            checksum += ChecksumFns.SumAsUShorts(packet.Data);
            return ChecksumFns.FinishChecksum(checksum);
        }


        public static string UDPToDebugString(IPAddress sourceAddr, IPAddress destAddr, UDPFrame frame)
        {
            var sb = new StringBuilder();

            var srcAddrWords = string.Join(" ", 
                ChecksumFns.DataBytesToShorts(sourceAddr.GetAddressBytes())
                        .Select(s => $"{s:x4}"));

            sb.AppendLine($"{srcAddrWords} source addr");

            var dstAddrWords = string.Join(" ", 
                ChecksumFns.DataBytesToShorts(destAddr.GetAddressBytes())
                        .Select(s => $"{s:x4}"));
            
            sb.AppendLine($"{dstAddrWords} dst addr");
            sb.AppendLine($"{17:x} protocol");
            sb.AppendLine($"{frame.Header.HeaderAndDataLength:x4} UDP length");
            sb.AppendLine($"{frame.Header.SourcePort:x4} source port");
            sb.AppendLine($"{frame.Header.DestinationPort:x4} dest port");
            sb.AppendLine($"{frame.Header.HeaderAndDataLength:x4} Length");
            sb.AppendLine($"{frame.Header.Checksum:x4} Checksum");
            sb.AppendLine($"DATA ({frame.Data.Length}B):");

            var byteStep = 8;

            for (int i=0; i<frame.Data.Length; i+=byteStep)
            {
                var words = ChecksumFns.DataBytesToShorts(frame.Data.Skip(i).Take(byteStep).ToArray());
                var wordLine = string.Join(" ", words.Select(s => $"{s:x4}"));
                sb.AppendLine(wordLine);
            }

            return sb.ToString();
        }
    }
}