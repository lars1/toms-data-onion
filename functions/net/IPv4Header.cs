using System.Net;

namespace Functions.Net
{
  public class IPv4Header
    {
        public int InternetHeaderLength { get; set; }
        public int TotalLength { get; set; }
        public IPAddress DestinationAddress  { get; set; }
        public IPAddress SourceAddress { get; set; }
        public int DatagramProtocol { get; set; }
        public bool ChecksumOK { get; set; }
        public ushort OnesComplementSum { get; set; }
    }
}