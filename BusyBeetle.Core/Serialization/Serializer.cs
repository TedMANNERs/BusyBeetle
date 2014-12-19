using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace BusyBeetle.Core.Serialization
{
    public class Serializer : ISerializer
    {
        public IPacket Deserialize(Stream stream)
        {
            if (stream == null)
                return null;

            int type = stream.ReadByte();
            switch (type)
            {
                case (int)PacketType.SizeNegotiation:
                    int[] size = new int[2];
                    size[0] = stream.ReadByte();
                    size[1] = stream.ReadByte();
                    return new Packet { Type = PacketType.SizeNegotiation, Content = size };

                case (int)PacketType.PixelData:
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
                    return new Packet { Type = PacketType.PixelData, Content = pixels };
            }
            return null;
        }

        public byte[] Serialize(IPacket input)
        {
            if (input == null)
                return null;

            switch (input.Type)
            {
                case PacketType.SizeNegotiation:
                    int[] size = (int[])input.Content;
                    byte[] sizeBytes = new byte[3];
                    sizeBytes[0] = (byte)PacketType.SizeNegotiation;
                    sizeBytes[1] = (byte)size[0];
                    sizeBytes[2] = (byte)size[1];
                    return sizeBytes;

                case PacketType.PixelData:
                    IList<PixelData> pixels = (List<PixelData>)input.Content;
                    int length = pixels.Count;

                    List<byte> bytes = new List<byte> { (byte)PacketType.PixelData };

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
            return null;
        }
    }
}