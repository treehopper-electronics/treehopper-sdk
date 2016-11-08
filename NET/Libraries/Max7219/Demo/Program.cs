using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries.Max7219;

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

            var display = new SevenSeg(board.Spi, board.Pins[5], 0, 10);

            int i = 0;
            while (!Console.KeyAvailable)
            {
                display.Text = i++;
            }

            board.Disconnect();
        }
    }
}
