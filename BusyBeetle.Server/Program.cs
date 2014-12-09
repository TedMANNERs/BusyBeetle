using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Threading;
using BusyBeetle.Core;

namespace BusyBeetle.Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("BusyBeetle Server");
            IDispatcher dispatcher = new BeetleDispatcher(Dispatcher.CurrentDispatcher);
            CoreKernel.Instance.Kernel.Bind<IDispatcher>().ToConstant(dispatcher).InSingletonScope();
            Service service = CoreKernel.Get<Service>();
            new Task(() => service.Start(IPAddress.Any, 6006)).Start();
            Console.WriteLine("Press [ENTER] to exit.");
            Console.ReadLine();
            service.Stop();
        }
    }
}