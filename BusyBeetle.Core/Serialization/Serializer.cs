﻿using System.Collections.Generic;
using System.Drawing;

namespace BusyBeetle.Core.Serialization
{
    public class Serializer : ISerializer
    {
        public IPacket Deserialize(byte[] bytes)
        {
            switch (bytes[0])
            {
                case (byte)PacketType.MoveCommand:
                    return new Packet { Type = PacketType.MoveCommand };

                case (byte)PacketType.IdExchange:
                    int id = bytes[1];
                    id += bytes[2] << 8;
                    id += bytes[3] << 16;
                    id += bytes[4] << 32;
                    return new Packet { Type = PacketType.IdExchange, Content = id };

                case (byte)PacketType.PixelData:
                {
                    int count = bytes[1];
                    int offset = 1;

                    if (count >= 0xFE)
                    {
                        count = bytes[2];
                        count += bytes[3] << 8;
                        count += bytes[4] << 16;
                        offset = 4;
                    }

                    IList<PixelData> pixels = new List<PixelData>();
                    for (int i = 1; i <= count; i++)
                    {
                        int posX = bytes[offset + 1];
                        int posY = bytes[offset + 2];
                        Color color = Color.FromArgb(bytes[offset + 3], bytes[offset + 4], bytes[offset + 5]);
                        pixels.Add(new PixelData(posX, posY, color));
                        offset += 5;
                    }
                    return new Packet { Type = PacketType.PixelData, Content = pixels };
                }
            }
            return null;
        }

        public byte[] Serialize(IPacket input)
        {
            if (input == null)
                return null;

            switch (input.Type)
            {
                case PacketType.MoveCommand:
                    return new[] { (byte)input.Type };

                case PacketType.IdExchange:
                    int id = (int)input.Content;
                    return new[] { (byte)input.Type, (byte)(id & 0xFF), (byte)((id >> 8) & 0xFF), (byte)((id >> 16) & 0xFF), (byte)((id >> 32) & 0xFF) };

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