using System;
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
                Console.WriteLine($"Pressure:    {sensor.Atm:0.00} Atm");
                Console.WriteLine($"Altitude:    {sensor.Altitude:0.00} m");
                Console.WriteLine($"Temperature: {sensor.Celsius:0.00} Celsius");
                
                // comment this line out if you're not using a sensor with humidity measurement
                Console.WriteLine($"Humidity:    {sensor.RelativeHumidity:0.00} % RH");
                Console.WriteLine();
                await Task.Delay(1000);
            }

            Console.WriteLine("Board disconnected");
        }
    }
}
