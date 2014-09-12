using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using BusyBeetle.Core;

namespace BusyBeetle.Client
{
    public class Service
    {
        public delegate void AppIdReceivedHandler(object sender, AppIdReceivedEventArgs e);

        private int _appId;
        private Coordinator _coordinator;
        private bool _isRunning;
        public event AppIdReceivedHandler OnAppIdReceivedHandler = delegate { };

        public void Start(string adress, int port, Coordinator coordinator)
        {
            _coordinator = coordinator;
            Connection.IsEstablished = false;
            _isRunning = true;
            do
            {
                Run(adress, port);
                Thread.Sleep(2000);
            }
            while (_isRunning);
        }

        private void Run(string adress, int port)
        {
            try
            {
                TcpClient client = new TcpClient(adress, port);
                NetworkStream stream = client.GetStream();
                do
                {
                    try
                    {
                        BinaryWriter writer = new BinaryWriter(stream);
                        BinaryReader reader = new BinaryReader(stream);

                        if (!Connection.IsEstablished)
                        {
                            writer.Write(_appId);
                            writer.Flush();
                            _appId = reader.ReadInt32();
                            OnAppIdReceivedHandler(this, new AppIdReceivedEventArgs { AppId = _appId });
                            long length = reader.ReadInt64();
                            byte[] image = new byte[length];
                            ServiceHelper.ReadBytesFromStream(stream, image);

                            Bitmap bitmap = new Bitmap(ByteArrayToBitmap(image));
                            _coordinator.World.Bitmap = bitmap;
                            Connection.IsEstablished = true;
                        }
                        string message = reader.ReadString();
                        if (!message.Equals("YourMove"))
                            continue;
                        ServiceHelper.AddPixels(reader, stream, _coordinator.World.SetAt);

                        List<PixelData> pixels = new List<PixelData>();
                        lock (_coordinator.World.Beetles)
                        {
                            foreach (Beetle beetle in _coordinator.World.Beetles)
                            {
                                beetle.Update();
                                pixels.Add(beetle.ModifiedPixel);
                            }
                        }

                        byte[] pixelArray;
                        if (_coordinator.World.Beetles.Any())
                        {
                            pixelArray = ServiceHelper.ObjectToByteArray(pixels);
                            writer.Write(pixelArray.Length);
                            writer.Write(pixelArray);
                        }
                        else
                        {
                            pixelArray = new byte[0];
                            writer.Write(pixelArray.Length);
                        }
                        writer.Flush();
                    }
                    catch (SocketException)
                    {
                        Connection.IsEstablished = false;
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
            catch (Exception)
            {
            }
        }

        private Bitmap ByteArrayToBitmap(byte[] image)
        {
            Bitmap bitmap;
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(image, 0, image.Length);
                bitmap = new Bitmap(stream);
            }
            return bitmap;
        }

        public void Stop()
        {
            _isRunning = false;
            Connection.IsEstablished = false;
        }
    }

    public class AppIdReceivedEventArgs : EventArgs
    {
        public int AppId { get; set; }
    }
}