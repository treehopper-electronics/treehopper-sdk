using System;
using System.Threading.Tasks;
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
                Console.WriteLine(
                    $"Current temperature: {sensor.Celsius:0.00} Celsius ({sensor.Fahrenheit:0.00} Fahrenheit)");
                await Task.Delay(1000);
            }

            board.Disconnect();
            Console.WriteLine("Board disconnected");
        }
    }
}
