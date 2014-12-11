namespace BusyBeetle.Core.Serialization
{
    public interface IPacket
    {
        PacketType Type { get; set; }
        object Content { get; set; }
    }
}