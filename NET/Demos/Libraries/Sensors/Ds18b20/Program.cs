using System;
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
            var sensors = await group.FindAllAsync();
            foreach (var sensor in sensors)
            {
                Console.WriteLine(sensor.Address);

                // disable auto-update so we can access multiple temperature properties without doing re-reads.
                // Consequently, we must explicitly call Update() to read the result
                sensor.AutoUpdateWhenPropertyRead = false; 
            }
            Console.WriteLine("\n");
            while (board.IsConnected)
            {
                Console.WriteLine("Starting sampling... (press any key to exit)");
                using (await group.StartConversionAsync()) // this triggers simultaneous conversion on all sensors
                {
                    foreach (var temp in sensors)
                    {
                        await temp.UpdateAsync(); // retrieve the conversion
                        Console.WriteLine(
                            $"Sensor {temp.Address} reports a temperature of {temp.Celsius} °C ({temp.Fahrenheit} °F)");
                    } 
                }

                Console.WriteLine("\n");
            }
        }
    }
}
