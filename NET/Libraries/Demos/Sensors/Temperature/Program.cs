using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Desktop;
using Treehopper.Libraries.Sensors.Temperature;

namespace TemperatureDemo
{
    /// <summary>
    /// This demo prints current temperature data every second. 
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            App();
        }

        static async Task App()
        {
            var board = await ConnectionService.Instance.GetFirstDeviceAsync();
            await board.ConnectAsync();

            // SENSOR SELECTION
            // Change this line depending on your temperature sensor
            var sensor = new Mcp9700(board.Pins[0], Mcp9700.Type.Mcp9701);
            //var sensor = new Lm75(board.I2c);
            //var sensor = new Mcp9808(board.I2c);

            Console.WriteLine("Starting temperature reading. Press any key to stop.");

            while (!Console.KeyAvailable)
            {
                Console.WriteLine(string.Format("Current temperature: {0:0.00} Celsius ({1:0.00} Fahrenheit)", sensor.Celsius, sensor.Fahrenheit));
                await Task.Delay(1000);
            }

            board.Disconnect();
            Console.WriteLine("Board disconnected");
        }
    }
}
