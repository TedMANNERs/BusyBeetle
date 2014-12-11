using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using BusyBeetle.Core.Serialization;

namespace BusyBeetle.Core
{
    public static class ServiceHelper
    {
        public static byte[] PacketToByteArray(IPacket packet)
        {
            if (packet == null)
                return null;
            Serializer serializer = new Serializer();
            return serializer.Serialize(packet);
        }

        public static IPacket ByteArrayToPacket(byte[] bytes)
        {
            if (bytes == null)
                return null;
            Serializer serializer = new Serializer();
            return serializer.Deserialize(bytes);
        }

        public static void ReadBytesFromStream(NetworkStream stream, byte[] bytes)
        {
            int streamResult;
            int offset = 0;
            do
            {
                streamResult = stream.Read(bytes, offset, bytes.Length - offset);
                offset += streamResult;
                if (offset == bytes.Length)
                    return;
            }
            while (streamResult != 0);
        }

        public static void AddPixels(BinaryReader reader, NetworkStream stream, Action<int, int, Color> action)
        {
            int arraySize = reader.ReadInt32();
            if (arraySize == 0)
                return;
            byte[] bytes = new byte[arraySize];
            ReadBytesFromStream(stream, bytes);
            List<PixelData> pixels = (List<PixelData>)ByteArrayToPacket(bytes).Content;
            foreach (PixelData pixel in pixels)
            {
                action(pixel.PositionX, pixel.PositionY, pixel.Color);
            }
        }

        public static void AddPixels(BinaryReader reader, NetworkStream stream, Action<PixelData> action)
        {
            int arraySize = reader.ReadInt32();
            if (arraySize == 0)
                return;
            byte[] bytes = new byte[arraySize];
            ReadBytesFromStream(stream, bytes);
            List<PixelData> pixels = (List<PixelData>)ByteArrayToPacket(bytes).Content;
            foreach (PixelData pixel in pixels)
            {
                action(pixel);
            }
            Console.WriteLine("Pixels received");
        }
    }
}