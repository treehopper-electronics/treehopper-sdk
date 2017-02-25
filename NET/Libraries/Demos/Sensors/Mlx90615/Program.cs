using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Desktop;
using Treehopper.Libraries.Sensors.Temperature;
namespace Mlx90615Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            App().Wait();
        }

        static async Task App()
        {
            var board = await ConnectionService.Instance.GetFirstDeviceAsync();
            await board.ConnectAsync();

            var tempSensor = new Mlx90615(board.I2c);

            while(true)
            {
                Console.Write("Ambient temperature: ");
                Console.WriteLine(tempSensor.Ambient.Fahrenheit);

                Console.Write("Object temperature: ");
                Console.WriteLine(tempSensor.Object.Fahrenheit);

                Console.Write("Raw IR data: ");
                Console.WriteLine(tempSensor.RawIrData);

                Console.WriteLine();

                await Task.Delay(1000);
            }
        }
    }
}
