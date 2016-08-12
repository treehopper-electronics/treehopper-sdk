using System;
using Treehopper;
using System.Threading.Tasks;
using Treehopper.Libraries;
using System.Diagnostics;
using System.Collections.Generic;

namespace Blink
{
    /// <summary>
    /// This demo blinks an LED attached to pin 1, using basic procedural programming.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Treehopper provides an asynchronous, event-driven API that helps your apps respond to plug / unplug detection.
    /// While this is the recommended way of working with TreehopperUsb references, many users new to C# and high-level languages
    /// in general may be uncomfortable with event-driven programming. To learn more about asynchronous usage, check out the
    /// BlinkEventDriven example.
    /// </para>
    /// <para>
    /// This example illustrates how to work with Treehopper boards procedurally to blink an LED. 
    /// </para>
    /// </remarks>
    class Program
    {
        static void Main(string[] args)
        {
            RunBlink();
        }

        static async void RunBlink()
        {
            while(true)
            {
                Console.Write("Waiting for board...");
                // Get a reference to the first TreehopperUsb board connected. This will await indefinitely until a board is connected.
                TreehopperUsb Board = await ConnectionService.Instance.First();        
                Console.WriteLine("Found board: " + Board);

                // You must explicitly open a board before communicating with it
                await Board.Connect();

                Board.Pins[0].Mode = PinMode.PushPullOutput;

                while(true)
                {
                    //Board.Led = !Board.Led;
                    Board.Pins[0].DigitalValue = !Board.Pins[0].DigitalValue;
                    await Task.Delay(1000);
                }


                // We arrive here when the board has been disconnected
                Console.WriteLine("Board has been disconnected.");
            }
            
        }
    }
}
