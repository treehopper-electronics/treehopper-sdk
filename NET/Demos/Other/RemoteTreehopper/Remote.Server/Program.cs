using System;
using System.Diagnostics;
using System.Threading;
using Treehopper;
using System.Collections.Specialized;

using System.Collections.Generic;
using uPLibrary.Networking.M2Mqtt;
using System.Net;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Text;

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
