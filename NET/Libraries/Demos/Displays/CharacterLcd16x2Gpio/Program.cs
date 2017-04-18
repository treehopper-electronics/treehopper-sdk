using System;
using System.Threading.Tasks;
using Treehopper.Desktop;
using Treehopper.Libraries.Displays;

namespace CharacterLcd16x2Gpio
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

            // configure the native parallel interface with the pins we're using
            board.ParallelInterface.RegisterSelectPin = board.Pins[0];
            board.ParallelInterface.ReadWritePin = board.Pins[1];
            board.ParallelInterface.EnablePin = board.Pins[2];
            board.ParallelInterface.DataBus.Add(board.Pins[3]);
            board.ParallelInterface.DataBus.Add(board.Pins[4]);
            board.ParallelInterface.DataBus.Add(board.Pins[5]);
            board.ParallelInterface.DataBus.Add(board.Pins[6]);
            board.ParallelInterface.DataBus.Add(board.Pins[7]);
            board.ParallelInterface.DataBus.Add(board.Pins[8]);
            board.ParallelInterface.DataBus.Add(board.Pins[9]);
            board.ParallelInterface.DataBus.Add(board.Pins[10]);

            var display = new Hd44780(board.ParallelInterface, 16, 2);
            await display.WriteLine("The time is:").ConfigureAwait(false);
            while (true)
            {
                await display.Write(DateTime.Now.ToLongTimeString()).ConfigureAwait(false);
                await display.SetCursorPosition(0, 1).ConfigureAwait(false);
                await Task.Delay(1000).ConfigureAwait(false);
            }
            
        }
    }
}
