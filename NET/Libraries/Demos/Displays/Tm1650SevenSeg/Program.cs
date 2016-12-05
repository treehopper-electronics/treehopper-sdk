using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries.Displays;

namespace Tm1650SevenSeg
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

            var controller = new Tm1650(board.I2c);
            var display = new SevenSegmentDisplay(controller.Leds);
            display.Text = 2345;
        }
    }
}
