using System.IO;

namespace BusyBeetle.Core.Serialization
{
    public interface ISerializer
    {
        byte[] Serialize(IPacket input);

        IPacket Deserialize(Stream stream);
    }
}