using System.Collections.Generic;

namespace BusyBeetle.Core.Serialization
{
    public interface ISerializer
    {
        byte[] Serialize(object input);

        object Deserialize(byte[] bytes);
    }
}