using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries.Displays;

namespace Max7219PixelDisplay
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

            board.Pins[19].Mode = PinMode.AnalogInput;

            var display = HobbyDisplayFactories.GetMax7219GraphicLedDisplay(board.Spi, board.Pins[9], 16);

            display.Brightness = 0.2;


            Console.WriteLine("Press any key to clear display and exit...");

            while (!Console.KeyAvailable)
            {
                await display.Print(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());
                await Task.Delay(250);
            }

            await display.Clear();

            board.Disconnect();

        }
    }
}
