namespace DearChar.Net.Tcp.Events
{
    internal struct EventData
    {
        public TcpChannel channel;
        public EventType eventType;
        public byte[] data;
    }
}
