using System;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries.Displays;
using Treehopper.Libraries.IO.PortExpander;

namespace CharacterLcd16x2Pcf8574
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
            var ioExpander = new Pcf8574(board.I2c, 8, false, false, false, 0x40);
            var display = HobbyDisplayFactories.GetCharacterDisplayFromPcf8574(ioExpander, 20, 4);
            await display.WriteLine("The current date is:").ConfigureAwait(false);
            while (true)
            {
                await display.WriteLine(DateTime.Now.ToLongTimeString()).ConfigureAwait(false);
                await display.WriteLine(DateTime.Now.ToShortDateString()).ConfigureAwait(false);
                await display.SetCursorPosition(0, 1);
                await Task.Delay(100);
            }


        }
    }
}
