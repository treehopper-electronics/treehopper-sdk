using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Desktop;
using Treehopper.Libraries.Sensors.Temperature;

namespace Ds18b20Test
{
    class Program
    {
        static TreehopperUsb board;
        static void Main(string[] args)
        {
            RunApp();
            Console.Read();
            board.Disconnect();
            Environment.Exit(0);
        }

        static async void RunApp()
        {
            Console.Write("Starting Ds18b20 temperature sensor demo...");
            board = await ConnectionService.Instance.GetFirstDeviceAsync();
            Console.WriteLine("Found board: " + board);
            await board.ConnectAsync();
            var group = new Ds18b20.Group(board.Uart);
            Console.WriteLine("Found temperature sensors at addresses:");
            var sensors = await group.FindAll();
            foreach (var addr in sensors)
            {
                Console.WriteLine(addr.Address);
            }
            Console.WriteLine("\n");
            while (board.IsConnected)
            {
                Console.WriteLine("Collecting readings... (press any key to exit)");
                using (await group.StartConversionAsync())
                {
                    foreach (var temp in sensors)
                    {
                        temp.AutoUpdateWhenPropertyRead = false;
                        await temp.Update();
                        Console.WriteLine(String.Format("Sensor {0} reports a temperature of {1} °C ({2} °F)", temp.Address, temp.Celsius, temp.Fahrenheit ));
                    }
                }

                Console.WriteLine("\n");
            }
        }
    }
}
