namespace Functions
{
    public static class ByteFns
    {
        public static ushort MakeUshort(byte hi, byte low) 
        {
            return (ushort) ((hi << 8) | low);
        }
    }
}