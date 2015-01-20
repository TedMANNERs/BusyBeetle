using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using BusyBeetle.Core;
using BusyBeetle.Core.Serialization;
using FluentAssertions;
using NUnit.Framework;

namespace BusyBeetle.CoreTest.Serialization
{
    [TestFixture]
    public class SerializerTest
    {
        [SetUp]
        public void Setup()
        {
            _testee = new Serializer();
        }

        private Serializer _testee;
        private const int PixelDataSize = 5;

        [Test]
        public void Deserialize_WhenStreamContainOnePixelData_ThenReturnPixelDataList()
        {
            // arrange
            byte[] bytes = { (byte)PacketType.PixelData, 0x01, 0x32, 0x32, 0x00, 0x00, 0x00 };
            MemoryStream stream = new MemoryStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
            stream.Position = 0;

            // act
            List<PixelData> pixels = (List<PixelData>)_testee.Deserialize(stream).Content;
            stream.Close();

            // assert
            pixels.Single().ShouldBeEquivalentTo(new PixelData(50, 50, Color.FromArgb(0x00, 0x00, 0x00)));
        }

        [Test]
        public void Deserialize_WhenStreamContainSeveralPixelData_ThenReturnPixelDataList()
        {
            // arrange
            byte[] bytes =
                {
                    (byte)PacketType.PixelData, 0x03,
                    0x32, 0x32, 0x00, 0x00, 0x00,
                    0x3C, 0x3C, 0xFF, 0xFF, 0xFF,
                    0x28, 0x20, 0x33, 0x33, 0xFF
                };
            MemoryStream stream = new MemoryStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
            stream.Position = 0;

            // act
            List<PixelData> pixels = (List<PixelData>)_testee.Deserialize(stream).Content;
            stream.Close();

            // assert
            pixels[0].ShouldBeEquivalentTo(new PixelData(50, 50, Color.FromArgb(0x00, 0x00, 0x00)));
            pixels[1].ShouldBeEquivalentTo(new PixelData(60, 60, Color.FromArgb(0xFF, 0xFF, 0xFF)));
            pixels[2].ShouldBeEquivalentTo(new PixelData(40, 32, Color.FromArgb(0x33, 0x33, 0xFF)));
        }

        [Test]
        public void Deserialize_WhenStreamContains300PixelData_ThenReturnPixelDataList()
        {
            // arrange
            const int PixelCount = 300;
            const int ByteCount = PixelCount * PixelDataSize + 5;
            byte[] bytes = new byte[ByteCount];
            bytes[0] = (byte)PacketType.PixelData;
            bytes[1] = (0xFe);
            bytes[2] = PixelCount & 0xFF;
            bytes[3] = (PixelCount >> 8) & 0xFF;
            bytes[4] = (PixelCount >> 16) & 0xFF;

            int offset = PixelDataSize;
            for (int i = 0; i < PixelCount; i++)
            {
                bytes[offset] = 50;
                bytes[offset + 1] = 50;
                bytes[offset + 2] = 0x33;
                bytes[offset + 3] = 0x33;
                bytes[offset + 4] = 0xFF;
                offset += PixelDataSize;
            }

            MemoryStream stream = new MemoryStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
            stream.Position = 0;

            // act
            List<PixelData> pixels = (List<PixelData>)_testee.Deserialize(stream).Content;
            stream.Close();

            // assert
            for (int i = 0; i < PixelCount; i++)
                pixels[i].ShouldBeEquivalentTo(new PixelData(50, 50, Color.FromArgb(0x33, 0x33, 0xFF)));
        }

        [Test]
        public void Deserialize_WhenStreamContainsSizeNegotiation_ThenReturnSize()
        {
            // arrange
            byte[] bytes = { (byte)PacketType.SizeNegotiation, 0xC8, 0x96 };
            int[] expectedSize = { 200, 150 };
            MemoryStream stream = new MemoryStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
            stream.Position = 0;

            // act
            int[] size = (int[])_testee.Deserialize(stream).Content;
            stream.Close();

            // assert
            size.ShouldAllBeEquivalentTo(expectedSize);
        }

        [Test]
        public void Serialize_WhenInputContains300PixelData_ThenReturnBytes()
        {
            // arrange
            const int PixelCount = 300;
            const int ByteCount = PixelCount * PixelDataSize + 5;

            List<PixelData> pixels = new List<PixelData>();
            for (int i = 0; i < PixelCount; i++)
            {
                pixels.Add(new PixelData(50, 50, Color.Red));
            }

            int offset = PixelDataSize;
            byte[] expectedBytes = new byte[ByteCount];
            expectedBytes[0] = (byte)PacketType.PixelData;
            expectedBytes[1] = (0xFe);
            expectedBytes[2] = PixelCount & 0xFF;
            expectedBytes[3] = (PixelCount >> 8) & 0xFF;
            expectedBytes[4] = (PixelCount >> 16) & 0xFF;

            for (int i = 0; i < PixelCount; i++)
            {
                expectedBytes[offset] = 50;
                expectedBytes[offset + 1] = 50;
                expectedBytes[offset + 2] = 0xFF;
                expectedBytes[offset + 3] = 0x00;
                expectedBytes[offset + 4] = 0x00;
                offset += PixelDataSize;
            }

            // act
            byte[] bytes = _testee.Serialize(new Packet { Type = PacketType.PixelData, Content = pixels });

            // assert
            bytes.ShouldAllBeEquivalentTo(expectedBytes);
        }

        [Test]
        public void Serialize_WhenInputContainsSeveralPixelData_ThenReturnBytes()
        {
            // arrange
            List<PixelData> pixels = new List<PixelData> { new PixelData(50, 50, Color.Black), new PixelData(100, 100, Color.Red), new PixelData(75, 75, Color.Blue) };
            byte[] expectedBytes =
                {
                    (byte)PacketType.PixelData, 0x03,
                    0x32, 0x32, 0x00, 0x00, 0x00,
                    0x64, 0x64, 0xFF, 0x00, 0x00,
                    0x4B, 0x4B, 0x00, 0x00, 0xFF
                };

            // act
            byte[] bytes = _testee.Serialize(new Packet { Type = PacketType.PixelData, Content = pixels });

            // assert
            bytes.ShouldAllBeEquivalentTo(expectedBytes);
        }

        [Test]
        public void Serialize_WhenInputContainsSizeNegotiation_ThenReturnBytes()
        {
            // arrange
            int[] size = { 200, 150 };
            byte[] expectedBytes = { (byte)PacketType.SizeNegotiation, 0xC8, 0x96 };

            // act
            byte[] bytes = _testee.Serialize(new Packet { Type = PacketType.SizeNegotiation, Content = size });

            // assert
            bytes.ShouldAllBeEquivalentTo(expectedBytes);
        }
    }
}