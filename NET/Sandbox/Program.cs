using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Desktop;
using Treehopper.Libraries.Sensors.Optical;

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

            var sensor = new Vcnl4010(board.I2c);
            sensor.AutoUpdateWhenPropertyRead = false;

            while (board.IsConnected && !Console.KeyAvailable)
			{
                await sensor.Update().ConfigureAwait(false);
                Console.WriteLine("lux: " + sensor.Lux);
                Console.WriteLine("prox: " + sensor.Inches);
                //Console.WriteLine(imu.Accelerometer);
                await Task.Delay(100);
			}

        }
    }
}
