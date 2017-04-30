using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Desktop;
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

			//board.Pins[0].Mode = PinMode.PushPullOutput;

            var imu = new Adxl345(board.I2c);

			while (board.IsConnected && !Console.KeyAvailable)
			{
			    Console.WriteLine(imu.Accelerometer);
			    await Task.Delay(100);
			}

        }
    }
}
