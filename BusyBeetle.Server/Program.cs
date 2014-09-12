using System;
using System.Net;
using System.Threading.Tasks;

namespace BusyBeetle.Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("BusyBeetle Server");
            Service service = new Service();
            new Task(() => service.Start(IPAddress.Any, 6006)).Start();
            Console.WriteLine("Press [ENTER] to exit.");
            Console.ReadLine();
            service.Stop();
        }
    }
}