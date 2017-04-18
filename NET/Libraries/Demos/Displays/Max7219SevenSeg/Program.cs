using System;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Desktop;
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
            var controller = new Max7219(board.Spi, board.Pins[9]);

            var display = new SevenSegmentDisplay(controller.Leds, true);

            int i = 0;
            while (!Console.KeyAvailable)
            {
                display.Text = i++;
                await Task.Delay(10);
            }

            board.Disconnect();
        }
    }
}
