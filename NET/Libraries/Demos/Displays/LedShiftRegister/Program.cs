using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Desktop;
using Treehopper.Libraries.Displays;

namespace LedShiftRegisterDemo
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
            Console.WriteLine("Connected to " + board);
            var driver = new LedShiftRegister(board.Spi, board.Pins[5], board.Pwm1);
            driver.Brightness = 0.01;
            var bar = new BarGraph(driver.Leds);
            while (!Console.KeyAvailable)
            {
                for (int i = 1; i < 10; i++)
                {
                    bar.Value = i / 10.0;
                    await Task.Delay(50);
                }
                for (int i = 10; i >= 0; i--)
                {
                    bar.Value = i / 10.0;
                    await Task.Delay(50);
                }
                await Task.Delay(100);
            }
            board.Disconnect();
        }
    }
}
