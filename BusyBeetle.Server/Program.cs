using System;
using System.Configuration;
using System.Net;
using BusyBeetle.Core;
using Configuration = BusyBeetle.Core.Configuration;

namespace BusyBeetle.Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("BusyBeetle Server");

            IService service = CoreKernel.Get<ServerService>();

            IConfiguration config = new Configuration(IPAddress.Parse(ConfigurationManager.AppSettings["IpAddress"]), Convert.ToInt32(ConfigurationManager.AppSettings["Port"]));
            service.Init(config);
            service.Start();
            Console.WriteLine("Press [ENTER] to exit.");
            Console.ReadLine();
            service.Stop();
        }
    }
}