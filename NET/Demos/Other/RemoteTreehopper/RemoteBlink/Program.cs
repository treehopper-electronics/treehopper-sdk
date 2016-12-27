using Remote.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteBlink
{
    class Program
    {
        static void Main(string[] args)
        {
            App().Wait();
        }

        static async Task App()
        {
            Console.WriteLine("Starting connection...");
            //var connection = new RemoteConnectionService("http://localhost:8080");
            var connection = new RemoteConnectionService("http://raspberrypi:8080");
            var board = await connection.GetFirstDeviceAsync();
            Console.WriteLine("Board found: {0}", board);
            await board.ConnectAsync();
            Console.WriteLine("Connected.");


            bool led = false;
            while(!Console.KeyAvailable)
            {
                //board.Led = !board.Led;
                await board.hub.Invoke("SetLed", board.board, led);
                led = !led;
                //await Task.Delay(100);
            }

            board.Disconnect();
        }

    }
}
