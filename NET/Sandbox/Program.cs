using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries;
namespace Sandbox
{
    /// <summary>
    /// This is the in-tree "sandbox" app that developers use to quickly test stuff out
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            App().Wait();
        }

        static async Task App()
        {
            var board = await ConnectionService.Instance.GetFirstDeviceAsync();
            await board.ConnectAsync();

            board.Pins[0].Mode = PinMode.PushPullOutput;

            while(board.IsConnected && !Console.KeyAvailable)
            {
                // do stuff
                board.Pins[0].ToggleOutput();
            }
        }
    }
}
