using System;
using System.Threading;

namespace Remote.Server
{
    class Program
    {
        static RemoteTreehopperServer server;
        static void Main(string[] args)
        {
            string url = "raspberrypi";
            server = new RemoteTreehopperServer(url);
            Console.WriteLine("Starting server. Connecting to " + url);
            server.Start();
            Console.WriteLine("Server started");
            Thread.Sleep(-1);
        }
    }
}
