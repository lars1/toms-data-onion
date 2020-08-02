namespace Functions.Net
{
  public class IPv4Frame
    {
        public IPv4Header Header { get; set; }

        public byte[] Data { get; set; }
    }
}