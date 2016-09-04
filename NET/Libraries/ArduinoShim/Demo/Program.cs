using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            App().Wait();
        }

        static async Task App()
        {
            var board = await ConnectionService.Instance.First();
            var blinkSketch = new Blink(board);
            blinkSketch.Serial.WriteRequested += (msg) => Console.Write(msg); // if we want Serial.write() to print to our screen, we do this
            blinkSketch.Run();
        }
    }
}
