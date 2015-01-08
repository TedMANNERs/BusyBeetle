using System;
using System.Collections.Generic;
using System.Drawing;
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
        private const int Height = 200;
        private const int Width = 200;
        private readonly object _lockObject = new object();
        private readonly List<PixelData> _modifiedPixels = new List<PixelData>();
        private readonly ISerializer _serializer;
        private Task _clientHandler;
        private IList<Client> _clients;
        private IConfiguration _config;
        private Task _connectionHandler;
        private bool _isRunning;
        private TcpListener _listener;
        private Color[][] _world;

        public ServerService(ISerializer serializer)
        {
            _serializer = serializer;
            InitPixelArray(Width, Height);
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

        private void InitPixelArray(int width, int height)
        {
            _world = new Color[width][];

            for (int i = 0; i < width; i++)
            {
                _world[i] = new Color[height];
                for (int j = 0; j < height; j++)
                {
                    _world[i][j] = Color.White;
                }
            }
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

                        for (int i = 0; i < Width; i++)
                        {
                            for (int j = 0; j < Height; j++)
                            {
                                pixels.Add(new PixelData(i, j, _world[i][j]));
                            }
                        }

                        int[] worldSize = { Width, Height };
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
                                    _world[pixel.PositionX][pixel.PositionY] = pixel.Color;
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