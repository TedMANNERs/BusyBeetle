using System;
using System.Net;
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
            Console.WriteLine("Press [ENTER] to exit.");
            Console.ReadLine();
            service.Stop();
        }
    }
}