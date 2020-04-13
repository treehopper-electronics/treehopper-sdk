using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Desktop.WinUsb;
using Treehopper.Libraries.Displays;

namespace Apa102Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            App2();
        }

        static async Task App2()
        {
            var board = await ConnectionService.Instance.GetFirstDeviceAsync();
            Console.WriteLine($"Connecting to {board} (Version: {board.VersionString})");
            await board.ConnectAsync();
            Console.WriteLine("Board connected. Starting rainbow demo");
            Console.WriteLine("Press any key to clear display and disconnect.");
            var driver = new Apa102(board.Spi, 1); // change to match the number of LEDs you have connected

            driver.Brightness = 1; 
            driver.AutoFlush = false; // only send updates when we explicitly Flush()

            int hueOffset = 0;

            while (!Console.KeyAvailable)
            {
                for (int i = 0; i < driver.Leds.Count; i++)
                {
                    //driver.Leds[i].SetRgb(0, 0, 255);
                    driver.Leds[i].SetHsl((360f / driver.Leds.Count) * i + hueOffset, 100, 50);
                }

                await driver.FlushAsync().ConfigureAwait(false);
                //await Task.Delay(10).ConfigureAwait(false);

                hueOffset += 1;
            }

            await driver.Clear();

            board.Disconnect();
            Console.WriteLine("Board disconnected");
        }
    }
}
