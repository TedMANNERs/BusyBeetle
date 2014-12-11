namespace BusyBeetle.Core.Serialization
{
    public class Packet : IPacket
    {
        public PacketType Type { get; set; }
        public object Content { get; set; }
    }
}