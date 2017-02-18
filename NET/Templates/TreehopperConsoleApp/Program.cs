using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Desktop;
using Treehopper.Libraries;

namespace TreehopperConsoleApp
{
    class Program
    {
        static TreehopperUsb board;
        static void Main(string[] args)
        {
            App().Wait();
            board?.Disconnect();
            Console.WriteLine("Board disconnected");
        }

        static async Task App()
        {
            board = await ConnectionService.Instance.GetFirstDeviceAsync();
            await board.ConnectAsync();
            Console.WriteLine("Connected to: " + board);
            Console.WriteLine("Firmware version: " + board.VersionString);
            
            // Write your code here

        }
    }
}
