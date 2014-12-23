using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using BusyBeetle.Core;
using BusyBeetle.Core.Serialization;

namespace BusyBeetle.Server
{
    public class ServerService : IService, IDisposable
    {
        private readonly object _lockObject = new object();
        private readonly List<PixelData> _modifiedPixels = new List<PixelData>();
        private readonly ISerializer _serializer;
        private readonly IWorld _world;
        private Task _clientHandler;
        private IList<Client> _clients;
        private IConfiguration _config;
        private Task _connectionHandler;
        private bool _isRunning;
        private TcpListener _listener;

        public ServerService(ISerializer serializer, IWorldFactory worldFactory)
        {
            _serializer = serializer;
            _world = worldFactory.Create(200, 200, false);
            _clients = new List<Client>();
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
            _connectionHandler = new Task(HandleConnections);
            _connectionHandler.Start();
            _clientHandler = new Task(HandleClients);
            _clientHandler.Start();
        }

        public void Stop()
        {
            _isRunning = false;
            _listener.Stop();
            foreach (Client client in _clients)
            {
                client.TcpClient.Close();
            }
            _connectionHandler.Wait();
            _clientHandler.Wait();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connectionHandler.Dispose();
                _clientHandler.Dispose();
            }
        }

        private void HandleConnections()
        {
            _listener = new TcpListener(_config.IpAddress, _config.Port);
            _listener.Start();
            do
            {
                try
                {
                    TcpClient tcpClient = _listener.AcceptTcpClient();
                    Console.WriteLine("Got new client");
                    lock (_lockObject)
                    {
                        Console.WriteLine("Locked connection handler");
                        if (_clients.Select(x => x.TcpClient).Contains(tcpClient))
                            continue;
                        NetworkStream stream = tcpClient.GetStream();
                        List<PixelData> pixels = new List<PixelData>();

                        for (int i = 0; i < _world.Width; i++)
                        {
                            for (int j = 0; j < _world.Height; j++)
                            {
                                pixels.Add(new PixelData(i, j, _world.GetAt(i, j)));
                            }
                        }

                        int[] worldSize = { _world.Width, _world.Height };
                        byte[] sizeBytes = _serializer.Serialize(new Packet { Type = PacketType.SizeNegotiation, Content = worldSize });
                        stream.Write(sizeBytes, 0, sizeBytes.Length);
                        stream.Flush();

                        byte[] pixelBytes = _serializer.Serialize(new Packet { Type = PacketType.PixelData, Content = pixels });
                        stream.Write(pixelBytes, 0, pixelBytes.Length);
                        stream.Flush();

                        lock (_clients)
                        {
                            _clients.Add(new Client(tcpClient));
                        }
                    }
                    Console.WriteLine("Unlocked connection handler");
                }
                catch (SocketException)
                {
                    _isRunning = false;
                }
            }
            while (_isRunning);
            _listener.Stop();
        }

        private void HandleClients()
        {
            while (_isRunning)
            {
                lock (_lockObject)
                {
                    lock (_clients)
                    {
                        foreach (Client client in _clients)
                        {
                            try
                            {
                                client.LastModifiedPixels.ForEach(x => _modifiedPixels.Remove(x));
                                client.LastModifiedPixels.Clear();

                                NetworkStream stream = client.TcpClient.GetStream();
                                byte[] bytes = _serializer.Serialize(new Packet { Type = PacketType.PixelData, Content = _modifiedPixels });
                                stream.Write(bytes, 0, bytes.Length);
                                stream.Flush();

                                IPacket packet = _serializer.Deserialize(stream);
                                List<PixelData> pixels = (List<PixelData>)packet.Content;
                                foreach (PixelData pixel in pixels)
                                {
                                    _modifiedPixels.Add(pixel);
                                    client.LastModifiedPixels.Add(pixel);
                                    _world.Bitmap.SetPixel(pixel.PositionX, pixel.PositionY, pixel.Color);
                                }
                            }
                            catch (IOException e)
                            {
                                _clients = new List<Client>(_clients.Where(c => c != client));
                                Console.WriteLine(e.Message);
                            }
                            catch (InvalidOperationException e)
                            {
                                _clients = new List<Client>(_clients.Where(c => c != client));
                                Console.WriteLine(e.Message);
                            }
                        }
                    }
                }
            }
        }
    }
}