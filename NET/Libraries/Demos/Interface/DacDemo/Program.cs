using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries.Interface.Dac;

namespace DacDemo
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

            var dac = new Mcp4725(board.I2c);
            Stopwatch sw = new Stopwatch();
            Console.WriteLine("Outputing sine wave. Press any key to stop");
            sw.Start();
            double frequency = 10;
            while(!Console.KeyAvailable)
            {
                dac.Value = Math.Sin(2.0 * Math.PI * (frequency * sw.Elapsed.TotalMilliseconds / 1000.0)) / 2.0 + 0.5;
            }
            Console.WriteLine("Disconnected");
        }
    }
}
