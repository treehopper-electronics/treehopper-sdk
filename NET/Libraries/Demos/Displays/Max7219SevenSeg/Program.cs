using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries.Displays;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            App().ConfigureAwait(false);
        }

        static async Task App()
        {
            var board = await ConnectionService.Instance.GetFirstDeviceAsync();
            await board.ConnectAsync();
            board.Pins[10].Mode = PinMode.AnalogInput;
            var controller = new Max7219(board.Spi, board.Pins[3]);

            var display = new SevenSegmentDisplay(controller.Leds, true);

            //var display2 = new SevenSeg(board.Spi, board.Pins[3], 1, 10);
            //var display3 = new SevenSeg(board.Spi, board.Pins[3], 2, 10);
            //var display4 = new SevenSeg(board.Spi, board.Pins[3], 3, 10);
            //var display5 = new SevenSeg(board.Spi, board.Pins[3], 4, 10);

            int i = 0;
            while (!Console.KeyAvailable)
            {
                display.Text = board.Pins[10].AnalogValue;
                //await Task.Delay(10);
                //display2.Text = i++;
                //await Task.Delay(10);
                //display3.Text = i++;
                //await Task.Delay(10);
                //display4.Text = i++;
                //await Task.Delay(10);
                //display5.Text = i++;
                //await Task.Delay(10);
            }

            board.Disconnect();
        }
    }
}
