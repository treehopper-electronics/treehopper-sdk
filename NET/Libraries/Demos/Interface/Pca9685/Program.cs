using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries.Displays;
using Treehopper.Libraries.Interface;
using Treehopper.Libraries.Motors;

namespace Pca9685Demo
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
            Console.WriteLine("Connected to " + board);

            var driver = new Pca9685(board.I2c);

            // connect the RGB LED as open-drain
            driver.InvertOutput = true;
            driver.OutputDrive = Pca9685.OutputDriveMode.OpenDrain;

            var ledDriver = new PwmLedDriver<PcaPin>(driver.Pins);

            var rgb = new RgbLed(ledDriver.Leds[0], ledDriver.Leds[1], ledDriver.Leds[2]);
            rgb.BlueGain = 0.7f;
            rgb.GreenGain = 0.95f;

            while (!Console.KeyAvailable)
            {
                for (int i = 0; i < 360; i++)
                {
                    rgb.SetHsl(i, 100, 50);
                    await Task.Delay(10);
                }
            }

            board.Disconnect();
            Console.WriteLine("Board disconnected");
        }
    }
}
