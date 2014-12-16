using System;
using System.Configuration;
using System.Net;
using System.Windows.Threading;
using BusyBeetle.Core;
using BusyBeetle.Core.Dispatcher;
using Configuration = BusyBeetle.Core.Configuration;

namespace BusyBeetle.Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("BusyBeetle Server");

            IDispatcher dispatcher = new BeetleDispatcher(Dispatcher.CurrentDispatcher);
            CoreKernel.Instance.Kernel.Bind<IDispatcher>().ToConstant(dispatcher).InSingletonScope();
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