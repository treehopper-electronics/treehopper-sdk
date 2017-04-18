using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Treehopper.Desktop;
using Treehopper.Libraries.Displays;

namespace Is31Fl3218Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            App();
        }

        static async Task App()
        {
            var board = await ConnectionService.Instance.GetFirstDeviceAsync();
            await board.ConnectAsync();

            var display = new Is31fl3218(board.I2c);
            display.AutoFlush = false;
            List<RgbLed> Leds = new List<RgbLed>();
            for (int i = 0; i < 6; i++)
                Leds.Add(new RgbLed(display.Leds.Skip(3 * i).Take(3).Reverse().ToList())
                {
                    AutoFlush = false,
                    BlueGain = 0.4f,
                });

            while(!Console.KeyAvailable)
            {
                for(int i=0;i<360;i+=6)
                {
                    for(int j=0;j<6;j++)
                    {
                        Leds[j].SetHsl(i + 30 * j % 360, 100, 50);
                    }
                    await display.Flush();
                    await Task.Delay(25);
                }
            }
        }
    }
}
