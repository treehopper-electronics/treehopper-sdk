using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries.Displays;
using Treehopper.Libraries.Input;

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
            var button = new Button(board.Pins[0]);
            button.OnPressed += Button_OnPressed;
            var display = new Is31fl3236(board.I2c, 100, false, board.Pins[8]);

            // turn down the current so we don't go blind
            for(int i = 0; i<36; i++)
            {
                display.Currents[i] = Is31fl3236.Current.QuarterImax;
            }

            display.AutoFlush = false;
            List<RgbLed> Leds = new List<RgbLed>();
            for (int i = 0; i < 12; i++)
                Leds.Add(new RgbLed(display.Leds.Skip(3 * i).Take(3).Reverse().ToList())
                {
                    AutoFlush = false,
                    BlueGain = 0.4f,
                });

            while(!Console.KeyAvailable)
            {
                for(int i=0;i<360;i+=12)
                {
                    for(int j=0;j<12;j++)
                    {
                        Leds[j].SetHsl(i + 30 * j % 360, 100, 50);
                    }
                    await display.FlushAsync();
                    await Task.Delay(25);
                }
            }
        }

        private static void Button_OnPressed(object sender, Button.ButtonPressedEventArgs e)
        {
            
        }
    }
}
