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