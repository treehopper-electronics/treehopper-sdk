using System;
using Treehopper;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Collections.Generic;

namespace Blink
{
    /// <summary>
    /// This demo blinks the built-in LED using basic procedural programming.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This example illustrates how to work with Treehopper boards procedurally to blink an LED. 
    /// </para>
    /// </remarks>
    class Program
    {
        static void Main(string[] args)
        {
            RunBlink().Wait();
        }

        static async Task RunBlink()
        {
            Utilities.Map(1, 5, 3, 2, 5);

            while (true)
            {
                Console.Write("Waiting for board...");
                // Get a reference to the first TreehopperUsb board connected. This will await indefinitely until a board is connected.

                //TreehopperManager manager = new TreehopperManager();
                //manager.BoardAdded += Manager_BoardAdded;

                TreehopperUsb Board = await ConnectionService.Instance.First();
                Console.WriteLine("Found board: " + Board);

                // You must explicitly open a board before communicating with it
                await Board.Connect();

                while (Board.IsConnected)
                {
                    // toggle the LED
                    Board.Led = !Board.Led;

                    // wait 500 ms
                    await Task.Delay(500);
                }

                // We arrive here when the board has been disconnected
                Console.WriteLine("Board has been disconnected.");
            }

        }

        private static void Manager_BoardAdded(TreehopperManager sender, TreehopperBoard board)
        {
            //throw new NotImplementedException();
        }
    }
}
