using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries.Interface.ShiftRegister;

namespace Sn74hc595Demo
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

            // connect the first shift register to our board.
            var shift1 = new Sn74hc595(board.Spi, board.Pins[6], 8);

            // connect the second one to the first one. 
            // Up to 255shift registers can be connected at once
            var shift2 = new Sn74hc595(shift1);

            while(true)
            {
                for (int i = 0; i < 8; i++)
                {
                    shift1.Pins[i].DigitalValue = true;
                    await Task.Delay(1);
                }

                for (int i = 0; i < 8; i++)
                {
                    shift2.Pins[i].DigitalValue = true;
                    await Task.Delay(1);
                }

                for (int i = 0; i < 8; i++)
                {
                    shift1.Pins[i].DigitalValue = false;
                    await Task.Delay(1);
                }

                for (int i = 0; i < 8; i++)
                {
                    shift2.Pins[i].DigitalValue = false;
                    await Task.Delay(1);
                }
            }
        }
    }
}
