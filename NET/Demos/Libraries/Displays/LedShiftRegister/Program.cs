using System;
using System.Linq;
using System.Threading.Tasks;
using Treehopper;
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
            var driver = new LedShiftRegister(board.Spi, board.Pins[9], board.Pwm1, LedShiftRegister.LedChannelCount.SixteenChannel, 0.1);
            var driver2 = new LedShiftRegister(driver, LedShiftRegister.LedChannelCount.SixteenChannel);
            driver.Brightness = 0.01;
            var leds = driver.Leds.Concat(driver2.Leds);
            var ledSet1 = leds.Take(8).Concat(leds.Skip(16).Take(8)).ToList();
            var ledSet2 = leds.Skip(8).Take(8).Reverse().Concat(leds.Skip(24).Take(8).Reverse()).ToList();
            
            var bar1 = new BarGraph(ledSet1);
            var bar2 = new BarGraph(ledSet2);
            var collection = new LedDisplayCollection();
            collection.Add(bar1);
            collection.Add(bar2);
            while (!Console.KeyAvailable)
            {
                for (int i = 1; i < 16; i++)
                {
                    bar1.Value = i / 16.0;
                    bar2.Value = i / 16.0;
                    await collection.FlushAsync();
                    await Task.Delay(10);
                }
                await Task.Delay(100);
                for (int i = 16; i >= 0; i--)
                {
                    bar1.Value = i / 16.0;
                    bar2.Value = i / 16.0;
                    await collection.FlushAsync();
                    await Task.Delay(10);
                }
                await Task.Delay(100);
            }
            board.Disconnect();
        }
    }
}
