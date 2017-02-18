using System;
using System.Collections.Generic;
$if$ ($targetframeworkversion$ >= 3.5)using System.Linq;
$endif$using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Desktop;
using Treehopper.Libraries;

namespace $safeprojectname$
{
    class Program
    {
    static TreehopperUsb board;
        static void Main(string[] args)
        {
            App().Wait();
            board.Disconnect();
            Console.WriteLine("Board disconnected");
    }

        static async Task App()
        {
            board = await ConnectionService.Instance.GetFirstDeviceAsync();
            await board.ConnectAsync();
            Console.WriteLine("Board connected: " + board);

        }
    }
}
