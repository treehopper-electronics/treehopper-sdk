using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries.Sensors.Pressure;

namespace BarometricPressure
{
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

            var sensor = new Bme280(board.I2c);
            //var sensor = new Bmp280(board.I2c);

            sensor.AutoUpdateWhenPropertyRead = false;

            Console.WriteLine("Press any key to disconnect");

            while(!Console.KeyAvailable)
            {
                await sensor.Update();
                Console.WriteLine(string.Format("Pressure:    {0:0.00} Atm", sensor.Atm));
                Console.WriteLine(string.Format("Altitude:    {0:0.00} m", sensor.Altitude));
                Console.WriteLine(string.Format("Temperature: {0:0.00} Celsius", sensor.Celsius));
                
                // comment this line out if you're not using a sensor with humidity measurement
                Console.WriteLine(string.Format("Humidity:    {0:0.00} % RH", sensor.RelativeHumidity));
                Console.WriteLine();
                await Task.Delay(1000);
            }

            Console.WriteLine("Board disconnected");
        }
    }
}
