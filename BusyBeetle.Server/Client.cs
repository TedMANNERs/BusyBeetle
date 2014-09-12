using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using BusyBeetle.Core;

namespace BusyBeetle.Server
{
    public class Client
    {
        public Client(IPAddress adress, int id, TcpClient tcpClient)
        {
            IpAddress = adress;
            Id = id;
            TcpClient = tcpClient;
            LastModifiedPixels = new List<PixelData>();
        }

        public IPAddress IpAddress { get; set; }
        public int Id { get; set; }
        public TcpClient TcpClient { get; set; }
        public List<PixelData> LastModifiedPixels { get; set; }
    }
}