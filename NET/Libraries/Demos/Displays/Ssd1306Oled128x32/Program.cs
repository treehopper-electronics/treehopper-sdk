using System;
using System.Threading.Tasks;
using Treehopper.Libraries.Displays;
using Treehopper.Desktop;

namespace Ssd1306Oled128x32
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
            var display = new Ssd1306(board.I2c);

            Console.WriteLine("Press any key to clear display and exit...");

            while (!Console.KeyAvailable)
            {
                await display.Print(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());
                await Task.Delay(250);
            }

            await display.Clear();
        }
    }
}
