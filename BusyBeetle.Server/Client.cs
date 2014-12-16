using System.Collections.Generic;
using System.Net.Sockets;
using BusyBeetle.Core;

namespace BusyBeetle.Server
{
    public class Client
    {
        public Client(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
            LastModifiedPixels = new List<PixelData>();
        }

        public TcpClient TcpClient { get; private set; }
        public List<PixelData> LastModifiedPixels { get; private set; }
    }
}