namespace BusyBeetle.Core.Serialization
{
    public interface ISerializer
    {
        byte[] Serialize(IPacket input);

        IPacket Deserialize(byte[] bytes);
    }
}