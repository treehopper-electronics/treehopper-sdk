using System;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries.IO.Mux;
using Treehopper.Libraries.Sensors.Temperature;

namespace I2cAnalogMuxDemo
{
    class Program
    {
        static TreehopperUsb board;
        static void Main(string[] args)
        {
            App().Wait();
            board.Disconnect();
            Console.WriteLine("Board disconnected");
        }

        static async Task App()
        {
            board = await ConnectionService.Instance.GetFirstDeviceAsync();
            await board.ConnectAsync();
            Console.WriteLine("Board connected: " + board);
            var mux = new I2cAnalogMux(board.I2c, board.Pins[0], board.Pins[1]);

            var temp1 = new Mlx90615(mux.Ports[0]);
            var temp2 = new Mlx90615(mux.Ports[1]);
            var temp3 = new Mlx90615(mux.Ports[2]);
            var temp4 = new Mlx90615(mux.Ports[3]);

            Console.WriteLine("Press any key to close");

            while(!Console.KeyAvailable)
            {
                Console.WriteLine("Temperature Sensor #1: " + temp1.Object);
                Console.WriteLine("Temperature Sensor #2: " + temp2.Object);
                Console.WriteLine("Temperature Sensor #3: " + temp3.Object);
                Console.WriteLine("Temperature Sensor #4: " + temp4.Object);
                await Task.Delay(1000);
            }

        }
    }
}
