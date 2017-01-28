using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries.Displays;

namespace Apa102Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            App2().Wait();
        }

        static async Task App2()
        {
            var board = ConnectionService.Instance.Boards.Where(b =>b.Name == "APA102").FirstOrDefault();
            await board.ConnectAsync();
            Console.WriteLine("Board connected. Starting rainbow demo");
            Console.WriteLine("Press any key to clear display and disconnect.");
            var driver = new Apa102(board.Spi, 28);

            driver.Brightness = 1; 
            driver.AutoFlush = false; // only send updates when we explicitly Flush()

            int hueOffset = 0;

            while (!Console.KeyAvailable)
            {
                for (int i = 0; i < driver.Leds.Count; i++)
                {
                    driver.Leds[i].SetHsl(i * 12 + hueOffset, 100, 50);
                }

                await driver.Flush();
                await Task.Delay(10);

                hueOffset += 1;
            }

            await driver.Clear();

            board.Disconnect();
            Console.WriteLine("Board disconnected");
        }
    }
}
