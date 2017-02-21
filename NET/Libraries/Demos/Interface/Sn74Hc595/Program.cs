using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries.Interface.PortExpander;
using Treehopper.Libraries.Displays;
using Treehopper.Utilities;

namespace Sn74hc595Demo
{
    class Program
    {

        static void Main(string[] args)
        {
            App().Wait();
        }

        static BarGraph bar1;
        static BarGraph bar2;
        static Pin adcPin;
        static async Task App()
        {
            var board = await ConnectionService.Instance.GetFirstDeviceAsync();
            await board.ConnectAsync();

            adcPin = board.Pins[10];
            adcPin.Mode = PinMode.AnalogInput;
            // connect the first shift register to our board.
            var shift1 = new Sn74hc595(board.Spi, board.Pins[6], 8);

            // connect the second one to the first one. 
            // Up to 255shift registers can be connected at once
            var shift2 = new Sn74hc595(shift1);

            var ledController1 = new GpioLedDriver<ShiftOutPin>(shift1.Pins, false, shift1);
            var ledController2 = new GpioLedDriver<ShiftOutPin>(shift2.Pins, false, shift2);
            var ledController3 = new LedShiftRegister(shift2, board.Pwm1);

            ledController3.Brightness = 0.5;

            var ledCollection = ledController1.Leds.Concat(ledController2.Leds).ToList();
            bar1 = new BarGraph(ledCollection);
            bar2 = new BarGraph(ledController3.Leds);

            led1().Forget();
            led2().Forget();



            //board.Disconnect();
        }

        static async Task led1()
        {
            while (!Console.KeyAvailable)
            {
                for (int i = 1; i < 10; i++)
                {
                    bar1.Value = i / 10.0;
                    await Task.Delay(50);
                }
                for (int i = 10; i >= 0; i--)
                {
                    bar1.Value = i / 10.0;
                    await Task.Delay(50);
                }
                await Task.Delay(200);
            }
        }

        static async Task led2()
        {
            while (!Console.KeyAvailable)
            {
                for (int i = 1; i < 10; i++)
                {
                    bar2.Value = i / 10.0;
                    await Task.Delay(10);
                }
                for (int i = 10; i >= 0; i--)
                {
                    bar2.Value = i / 10.0;
                    await Task.Delay(10);
                }
            }
        }
    }
}
