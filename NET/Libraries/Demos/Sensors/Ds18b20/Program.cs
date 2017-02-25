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
            await Ds18b20.FindAll(board.Uart);
            Console.WriteLine("Found temperature sensors at addresses:");
            foreach(var addr in Ds18b20.AddressList)
            {
                Console.WriteLine(addr);
            }
            Console.WriteLine("\n");
            while (board.IsConnected)
            {
                try
                {
                    Console.WriteLine("Collecting readings... (press any key to exit)");
                    Dictionary<ulong, double> temps = await Ds18b20.GetAllTemperatures(board.Uart);
                    foreach (KeyValuePair<ulong, double> item in temps)
                    {
                        Console.WriteLine(String.Format("Sensor {0} reports a temperature of {1} °C ({2} °F)", item.Key, item.Value, Ds18b20.CelsiusToFahrenheit(item.Value)));
                    }
                    Console.WriteLine("\n");
                } catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
