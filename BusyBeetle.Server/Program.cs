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
            IService service = CoreKernel.Get<ServerService>();
            GameType gameType;
            Enum.TryParse(ConfigurationManager.AppSettings["GameType"], out gameType);
            IConfiguration config = new Configuration(IPAddress.Parse(ConfigurationManager.AppSettings["IpAddress"]), Convert.ToInt32(ConfigurationManager.AppSettings["Port"]), gameType);

            Console.WriteLine(config.GameType + " Server");

            service.Init(config);
            service.Start();
            Console.WriteLine("Press [ENTER] to exit.");
            Console.ReadLine();
            service.Stop();
        }
    }
}