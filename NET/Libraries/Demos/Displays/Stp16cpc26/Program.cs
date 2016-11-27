using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries.LedDrivers;

namespace Stp16cpc26Demo
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
            var driver = new Stp16cpc26(board.Spi, board.Pins[6], board.Pwm1);
            driver.Brightness = 0.01;
            driver.AutoFlush = false;
            var bar = new BarGraph(driver.Leds);
            board.Pins[9].Mode = PinMode.AnalogInput;
            while (!Console.KeyAvailable)
            {
                Console.WriteLine(board.Pins[9].AnalogValue);
                bar.Value = board.Pins[9].AnalogValue;
                await Task.Delay(10);
                //await Task.Delay(100);
                //for (int i = 1; i < 10; i++)
                //{
                //    bar.Value = i / 10.0;
                //    await Task.Delay(50);
                //}
                //for (int i = 10; i > 0; i--)
                //{
                //    bar.Value = i / 10.0;
                //    await Task.Delay(50);
                //}

            }
            board.Disconnect();

        }
    }
}
