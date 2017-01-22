using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries.Sensors.Temperature;

namespace Lm75Demo
{
    /// <summary>
    /// This demo prints current temperature data every second
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
            var temp = new Lm75(board.I2c);

            Console.WriteLine("Starting temperature reading. Press any key to stop.");

            while (!Console.KeyAvailable)
            {
                Console.WriteLine(string.Format("Current temperature: {0:0.00} Celsius ({1:0.00} Fahrenheit)", temp.TemperatureCelsius, temp.TemperatureFahrenheit));
                await Task.Delay(1000);
            }

            board.Disconnect();
            Console.WriteLine("Board disconnected");
        }
    }
}
