﻿using System;
using Treehopper;
using System.Threading.Tasks;
using Treehopper.Desktop;
using System.Windows.Threading;

namespace Blink
{
    /// <summary>
    /// This demo blinks the built-in LED using async programming.
    /// </summary>
    class Program
    {
        static TreehopperUsb Board;

        //[MTAThread]
        static void Main(string[] args)
        {
            // We can't do async calls from the Console's Main() function, so we run all our code in a separate async method.
            Task.Run(RunBlink).Wait();
        }

        static async Task RunBlink()
        {
            while (true)
            {
                Console.Write("Waiting for board...");
                // Get a reference to the first TreehopperUsb board connected. This will await indefinitely until a board is connected.
                Board = await ConnectionService.Instance.GetFirstDeviceAsync();
				Console.WriteLine("Found board: " + Board);
				Console.WriteLine("Version: " + Board.VersionString);

				// You must explicitly connect to a board before communicating with it
				await Board.ConnectAsync();

				Console.WriteLine("Start blinking. Press any key to stop.");
				while (Board.IsConnected)
				{
                    if (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                        Board.Disconnect();
                        break;
                    } else
                    {
                        // toggle the LED.
                        Board.Led = !Board.Led;
                        await Task.Delay(100);
                    }

				}

                
            }
        }
    }
}
