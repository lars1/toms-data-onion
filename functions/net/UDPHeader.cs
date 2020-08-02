namespace Functions.Net
{
  public class UDPHeader
    {
        public ushort SourcePort { get; set; }
        public ushort DestinationPort { get; set; }
        public ushort HeaderAndDataLength { get; set; }
        public ushort Checksum { get; set; }
    }

}