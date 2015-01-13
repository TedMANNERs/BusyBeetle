using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using BusyBeetle.Core;
using BusyBeetle.Core.Serialization;

namespace BusyBeetle.Client
{
    public class ClientService : IService, IDisposable
    {
        private readonly ICoordinator _coordinator;
        private readonly ISerializer _serializer;
        private readonly IWorldFactory _worldFactory;
        private IConfiguration _config;
        private Task _connectionHandler;
        private bool _isRunning;

        public ClientService(ISerializer serializer, ICoordinator coordinator, IWorldFactory worldFactory)
        {
            _serializer = serializer;
            _coordinator = coordinator;
            _worldFactory = worldFactory;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Init(IConfiguration config)
        {
            _config = config;
        }

        public void Start()
        {
            _isRunning = true;
            _connectionHandler = new Task(HandleConnection);
            _connectionHandler.Start();
        }

        public void Stop()
        {
            _isRunning = false;
            Connection.IsEstablished = false;
            _connectionHandler.Wait();
            _connectionHandler.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connectionHandler.Dispose();
            }
        }

        private void HandleConnection()
        {
            do
            {
                if (!Connection.IsEstablished)
                    Run();
                Thread.Sleep(1000);
            }
            while (_isRunning);
        }

        private void Run()
        {
            try
            {
                TcpClient client = new TcpClient(_config.IpAddress.ToString(), _config.Port);
                NetworkStream stream = client.GetStream();
                Connection.IsEstablished = true;

                IPacket gameTypePacket = _serializer.Deserialize(stream);
                GameType gameType = (GameType)gameTypePacket.Content;

                IPacket worldSizePacket = _serializer.Deserialize(stream);
                int[] worldSize = (int[])worldSizePacket.Content;

                _coordinator.World = _worldFactory.Create(worldSize[0], worldSize[1], gameType);

                IPacket initialWorldPacket = _serializer.Deserialize(stream);
                List<PixelData> initialPixels = (List<PixelData>)initialWorldPacket.Content;
                foreach (PixelData pixel in initialPixels)
                {
                    _coordinator.World.SetAt(pixel.PositionX, pixel.PositionY, pixel.Color);
                }

                do
                {
                    try
                    {
                        IPacket packet = _serializer.Deserialize(stream);
                        List<PixelData> pixels = (List<PixelData>)packet.Content;
                        foreach (PixelData pixel in pixels)
                        {
                            _coordinator.World.SetAt(pixel.PositionX, pixel.PositionY, pixel.Color);
                        }

                        byte[] packetBytes;
                        lock (_coordinator.World.Beetles)
                        {
                            if (_coordinator.World.Beetles.Any() || _coordinator.World.GameType == GameType.GameOfLife)
                            {
                                List<PixelData> modifiedPixels = _coordinator.World.Tick();
                                packetBytes = _serializer.Serialize(new Packet { Type = PacketType.PixelData, Content = modifiedPixels });
                            }
                            else
                            {
                                packetBytes = _serializer.Serialize(new Packet { Type = PacketType.PixelData, Content = new List<PixelData>() });
                            }
                        }

                        stream.Write(packetBytes, 0, packetBytes.Length);
                        stream.Flush();
                    }
                    catch (IOException)
                    {
                        Connection.IsEstablished = false;
                    }
                }
                while (Connection.IsEstablished);
                stream.Close();
                client.Close();
            }
            catch (SocketException)
            {
                Connection.IsEstablished = false;
            }
        }
    }
}