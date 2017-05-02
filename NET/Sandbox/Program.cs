using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Desktop;
using Treehopper.Libraries.IO.Adc;
using Treehopper.Libraries.Sensors.Inertial;

namespace Sandbox
{
    /// <summary>
    /// This is the in-tree "sandbox" app that developers use to quickly test stuff out
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
			App().Wait();
        }


        static async Task App()
        {
			var board = await ConnectionService.Instance.GetFirstDeviceAsync();

			await board.ConnectAsync();

            var adc = new Nau7802(board.I2c);
            //board.Pins[0].Mode = PinMode.PushPullOutput;

            //var imu = new Adxl345(board.I2c);
            long avg = 0;
            Console.WriteLine("taring, please wait...");
            for (int i = 0; i < 50; i++)
            {
                avg += adc.AdcValue;
                await Task.Delay(100);
            }

            avg /= 50;



            while (board.IsConnected && !Console.KeyAvailable)
			{
                Console.WriteLine(adc.AdcValue - avg);
                //Console.WriteLine(imu.Accelerometer);
                await Task.Delay(100);
			}

        }
    }
}
