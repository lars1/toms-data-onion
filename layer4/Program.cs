using System;
using System.IO;
using System.Diagnostics;
using System.Net;
using Functions;
using Functions.Net;
using System.Text;

namespace layer4
{
    class Program
    {
        static IPAddress FilterSourceAddr = IPAddress.Parse("10.1.1.10");
        static IPAddress FilterDestAddr = IPAddress.Parse("10.1.1.200");
        static ushort FilterDestPort = 42069;


        static bool PacketFilter(IPv4Header ipv4Header, UDPFrame udpFrame)
        {
            if (ipv4Header.ChecksumOK == false
                || !ipv4Header.SourceAddress.Equals(FilterSourceAddr)
                || !ipv4Header.DestinationAddress.Equals(FilterDestAddr)
                || udpFrame.Header.DestinationPort != FilterDestPort)
                return false;
            
            return UDPFns.UDPChecksum(ipv4Header.SourceAddress, ipv4Header.DestinationAddress, udpFrame) == udpFrame.Header.Checksum;
        }


        static int Main(string[] args)
        {
            if (args.Length < 1) 
            {
                Console.WriteLine(PuzzleFileFns.StandardOneParamRequiredErrorMessage);
                return 1;
            }

            var allLines = File.ReadAllLines(args[0]);
            int payloadStartLine = PuzzleFileFns.GetLinePayloadStartsAt(allLines);
            var payload = PuzzleFileFns.GetPayloadAsOneLongString(allLines, payloadStartLine);

            Debug.Assert(payload.StartsWith("<~70!<j!!#7a+;"));
            Debug.Assert(payload.EndsWith(":j*\"<<*=Q1C=~>"));
            Debug.Assert(payload.Contains("\n") == false);

            var ipv4PacketsSpan = Ascii85Fns.DecodeAsSpan(payload, 2);
            var packetStream = new MemoryStream(ipv4PacketsSpan.ToArray());
            var memBuffer = new MemoryStream();

            foreach (var ipv4Frame in IPv4Fns.IterateFrames(packetStream))
            {
                if (!ipv4Frame.Header.ChecksumOK) continue;

                var udpFrame = UDPFns.DecodeUDP(ipv4Frame.Data);

                if (!PacketFilter(ipv4Frame.Header, udpFrame)) continue;

                memBuffer.Write(udpFrame.Data, 0, udpFrame.Data.Length);
            }

            // Debugging:
            // var ipv4frame = IPv4Fns.IterateFrames(payloadStream).Last();
            
            // System.Console.WriteLine(
            //     UDPFns.UDPToDebugString(
            //         ipv4frame.Header.SourceAddress,
            //         ipv4frame.Header.DestinationAddress,
            //         UDPFns.DecodeUDP(ipv4frame.Data)));

            // var udpPacket = UDPFns.DecodeUDP(ipv4frame.Data);
            // var checksum = UDPFns.UDPChecksum(ipv4frame.Header.SourceAddress, ipv4frame.Header.DestinationAddress, udpPacket);
            // System.Console.WriteLine("Checksum (calc)={0:x4}", checksum);
            memBuffer.Position = 0L;
            Console.WriteLine(Encoding.ASCII.GetString(memBuffer.ToArray()));

            return 0;
        }
    }
}
