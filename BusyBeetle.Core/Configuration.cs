using System.Net;

namespace BusyBeetle.Core
{
    public class Configuration : IConfiguration
    {
        public Configuration(IPAddress ipAddress, int port)
        {
            IpAddress = ipAddress;
            Port = port;
        }

        public IPAddress IpAddress { get; private set; }
        public int Port { get; private set; }
    }
}