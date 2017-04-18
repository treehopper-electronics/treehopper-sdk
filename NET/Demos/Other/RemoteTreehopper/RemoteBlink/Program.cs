using Remote.Client;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Treehopper.Libraries.Sensors.Temperature;
using Treehopper.Utilities;

namespace RemoteBlink
{
    class Program
    {
        static void Main(string[] args)
        {
            App();
        }

        static async Task App()
        {
            Console.WriteLine("Starting connection...");
            var connection = new RemoteConnectionService("raspberrypi");
            var board = await connection.GetFirstDeviceAsync();
            Console.WriteLine("Board found: {0}", board);
            board.Open();

            var temp = new Mlx90615(board.I2c);
            Console.WriteLine("Connected.");
            Stopwatch sw = new Stopwatch();
            Task.Run(async() =>
            {
                while (!Console.KeyAvailable)
                {
                    Console.WriteLine("the current temperature is: " + temp.Ambient.Fahrenheit);
                    await Task.Delay(1000);
                }
                board.Disconnect();
            }).Forget();
        }

    }
}
