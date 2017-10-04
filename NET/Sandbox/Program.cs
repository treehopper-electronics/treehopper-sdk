using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            App();
        }


        static async Task App()
        {
			var board = await ConnectionService.Instance.GetFirstDeviceAsync();

			await board.ConnectAsync();
            board.Spi.Enabled = true;
            //var adc = new Nau7802(board.I2c);
            ////board.Pins[0].Mode = PinMode.PushPullOutput;

            ////var imu = new Adxl345(board.I2c);
            //long avg = 0;
            //Console.WriteLine("taring, please wait...");
            //for (int i = 0; i < 50; i++)
            //{
            //    avg += adc.AdcValue;
            //    await Task.Delay(100);
            //}

            //avg /= 50;
            var header = new byte[] { 0x00, 0x00, 0x00, 0x00 };
            var footer = new byte[] { 0xff, 0xff, 0xff, 0xff };
            var bytes = new List<byte>();

            var toggle = false;

            while (board.IsConnected && !Console.KeyAvailable)
			{
                board.Led = !board.Led;
                var message = header.Concat(bytes).ToArray();

                var led1 = new byte[] { 0x70, 0x00, 0x00, 0x00 };
                var led2 = new byte[] { 0x70, 0x00, 0x00, 0x00 };
                if (toggle)
                {
                    led1[0] = 0xff;
                    led1[1] = 0xff;
                    led1[2] = 0xff;
                    led1[3] = 0xff;

                    led2[0] = 0xff;
                    led2[1] = 0xff;
                    led2[2] = 0xff;
                    led2[3] = 0xff;
                }
                message = message.Concat(led1).ToArray();
                message = message.Concat(led2).ToArray();

                message = message.Concat(footer).ToArray();

                toggle = !toggle;

                //Console.WriteLine(adc.AdcValue - avg);
                //Console.WriteLine(imu.Accelerometer);
                await board.Spi.SendReceive(message, null, ChipSelectMode.SpiActiveLow, 0.1, SpiBurstMode.BurstTx,
                    SpiMode.Mode11);

                Task.Delay(10).Wait();
			}

        }
    }
}
