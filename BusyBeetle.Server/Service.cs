using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using BusyBeetle.Core;
using BusyBeetle.Core.Serialization;

namespace BusyBeetle.Server
{
    public class Service
    {
        private readonly List<PixelData> _modifiedPixels = new List<PixelData>();
        private readonly IWorld _world;
        private int _appId;
        private List<Client> _clients = new List<Client>();
        private bool _isRunning;

        public Service(IWorldFactory worldFactory)
        {
            _world = worldFactory.Create((int)(200 * Values.Scalefactor), (int)(200 * Values.Scalefactor));
        }

        public void Start(IPAddress adress, int port)
        {
            TcpListener listener = new TcpListener(adress, port);
            _isRunning = true;
            new Task(HandleClients).Start();
            listener.Start();
            do
            {
                try
                {
                    TcpClient tcpClient = listener.AcceptTcpClient();
                    if (_clients.Select(x => x.TcpClient).Contains(tcpClient))
                        continue;
                    NetworkStream stream = tcpClient.GetStream();
                    IPAddress clientAdress = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address;
                    BinaryReader reader = new BinaryReader(stream);
                    int readAppId = reader.ReadInt32();
                    //Register client
                    int newAppId = readAppId;

                    if (readAppId > _appId)
                        _appId = readAppId;
                    else if (readAppId == 0)
                    {
                        _appId++;
                        newAppId = _appId;
                    }

                    BinaryWriter writer = new BinaryWriter(stream);
                    writer.Write(newAppId);
                    byte[] bitmapBytes = BitmapToBytes(_world.Bitmap);
                    writer.Write(bitmapBytes.LongLength);
                    writer.Write(bitmapBytes);
                    writer.Flush();
                    lock (_clients)
                    {
                        _clients.Add(new Client(clientAdress, newAppId, tcpClient));
                    }
                    Console.WriteLine("New client {0}, {1} registered", clientAdress, newAppId.ToString("D5"));
                }
                catch (SocketException)
                {
                    _isRunning = false;
                }
            }
            while (_isRunning);
            listener.Stop();
        }

        public void HandleClients()
        {
            do
            {
                lock (_clients)
                {
                    foreach (Client client in _clients)
                    {
                        try
                        {
                            NetworkStream stream = client.TcpClient.GetStream();
                            BinaryWriter writer = new BinaryWriter(stream);
                            BinaryReader reader = new BinaryReader(stream);

                            client.LastModifiedPixels.ForEach(x => _modifiedPixels.Remove(x));
                            client.LastModifiedPixels.Clear();

                            //Send move permission and all modified pixels
                            writer.Write("YourMove");
                            byte[] pixelArray;
                            if (_modifiedPixels.Any())
                            {
                                pixelArray = ServiceHelper.PacketToByteArray(new Packet { Type = PacketType.PixelData, Content = _modifiedPixels });
                                writer.Write(pixelArray.Length);
                                writer.Write(pixelArray);
                                Console.WriteLine("Pixels to {0}; {1} sent", client.IpAddress, client.Id);
                            }
                            else
                            {
                                pixelArray = new byte[0];
                                writer.Write(pixelArray.Length);
                            }
                            writer.Flush();

                            //Receive modified pixels
                            //ServiceHelper.AddPixels(reader, stream, _modifiedPixels.Add);
                            int arraySize = reader.ReadInt32();
                            if (arraySize == 0)
                                continue;
                            byte[] bytes = new byte[arraySize];
                            ServiceHelper.ReadBytesFromStream(stream, bytes);
                            List<PixelData> pixels = (List<PixelData>)ServiceHelper.ByteArrayToPacket(bytes).Content;
                            foreach (PixelData pixel in pixels)
                            {
                                _modifiedPixels.Add(pixel);
                                client.LastModifiedPixels.Add(pixel);
                                _world.Bitmap.SetPixel(pixel.PositionX, pixel.PositionY, pixel.Color);
                            }
                            Console.WriteLine("Pixels from {0}; {1} received", client.IpAddress, client.Id);
                        }
                        catch (SocketException)
                        {
                            Stop();
                        }
                        catch (IOException e)
                        {
                            _clients = new List<Client>(_clients.Where(c => c != client));
                            Console.WriteLine(e.Message);
                            Console.WriteLine("Client {0}; {1} removed", client.IpAddress, client.Id);
                        }
                        catch (InvalidOperationException e)
                        {
                            _clients = new List<Client>(_clients.Where(c => c != client));
                            Console.WriteLine(e.Message);
                            Console.WriteLine("Client {0}; {1} removed", client.IpAddress, client.Id);
                        }
                    }
                }
            }
            while (_isRunning);
        }

        private static byte[] BitmapToBytes(Bitmap bitmap)
        {
            byte[] byteArray;
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                byteArray = stream.ToArray();
            }
            return byteArray;
        }

        public void Stop()
        {
            _isRunning = false;
            foreach (Client client in _clients)
            {
                client.TcpClient.Close();
            }
        }
    }
}