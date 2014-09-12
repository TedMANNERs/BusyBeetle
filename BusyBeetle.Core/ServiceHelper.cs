using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace BusyBeetle.Core
{
    public static class ServiceHelper
    {
        public static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, obj);
            return stream.ToArray();
        }

        public static Object ByteArrayToObject(byte[] bytes)
        {
            if (bytes == null)
                return null;
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            Object obj = formatter.Deserialize(stream);
            return obj;
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
            List<PixelData> pixels = (List<PixelData>)ByteArrayToObject(bytes);
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
            List<PixelData> pixels = (List<PixelData>)ByteArrayToObject(bytes);
            foreach (PixelData pixel in pixels)
            {
                action(pixel);
            }
            Console.WriteLine("Pixels received");
        }
    }
}