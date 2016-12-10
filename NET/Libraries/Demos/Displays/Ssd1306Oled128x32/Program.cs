using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Windows.UI.Xaml.Media.Imaging;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using Treehopper.Libraries.Displays;

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
