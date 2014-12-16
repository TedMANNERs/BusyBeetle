using System.Net;

namespace BusyBeetle.Core
{
    public interface IConfiguration
    {
        IPAddress IpAddress { get; }
        int Port { get; }
    }
}