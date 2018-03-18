using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Desktop;
using Treehopper.Libraries.Sensors.Optical;

namespace Sandbox
{
    /// <summary>
    /// This is the in-tree "sandbox" app that developers use to quickly test stuff out
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            App();
        }

        static async Task App()
        {
			var board = await ConnectionService.Instance.GetFirstDeviceAsync();
			await board.ConnectAsync();

            while (board.IsConnected && !Console.KeyAvailable)
			{
                Task.Delay(10).Wait();
			}

        }
    }
}
