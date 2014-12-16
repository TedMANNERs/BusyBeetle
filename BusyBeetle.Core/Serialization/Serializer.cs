using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace BusyBeetle.Core.Serialization
{
    public class Serializer : ISerializer
    {
        public IPacket Deserialize(Stream stream)
        {
            int count = stream.ReadByte();

            if (count >= 0xFE)
            {
                count = stream.ReadByte();
                count += stream.ReadByte() << 8;
                count += stream.ReadByte() << 16;
            }

            IList<PixelData> pixels = new List<PixelData>();
            for (int i = 0; i < count; i++)
            {
                int posX = stream.ReadByte();
                int posY = stream.ReadByte();
                Color color = Color.FromArgb(stream.ReadByte(), stream.ReadByte(), stream.ReadByte());
                pixels.Add(new PixelData(posX, posY, color));
            }
            return new Packet { Content = pixels };
        }

        public byte[] Serialize(IPacket input)
        {
            if (input == null)
                return null;

            IList<PixelData> pixels = (List<PixelData>)input.Content;
            int length = pixels.Count;

            List<byte> bytes = new List<byte>();

            if (length >= 0xFE)
            {
                bytes.Add(0xFe);
                bytes.Add((byte)(length & 0xFF));
                bytes.Add((byte)((length >> 8) & 0xFF));
                bytes.Add((byte)((length >> 16) & 0xFF));
            }
            else
            {
                bytes.Add((byte)length);
            }

            foreach (PixelData pixel in pixels)
            {
                bytes.Add((byte)pixel.PositionX);
                bytes.Add((byte)pixel.PositionY);
                bytes.Add(pixel.Color.R);
                bytes.Add(pixel.Color.G);
                bytes.Add(pixel.Color.B);
            }

            return bytes.ToArray();
        }
    }
}