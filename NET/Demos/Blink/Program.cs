using System;
using Treehopper;
using System.Threading;
namespace Blink
{
    /// <summary>
    /// This demo blinks an LED attached to pin 1, using basic procedural programming.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Treehopper provides an asynchronous, event-driven API that helps your apps respond to plug / unplug detection.
    /// While this is the recommended way of working with TreehopperUSB references, many users new to C# and high-level languages
    /// in general may be uncomfortable with event-driven programming. To learn more about asynchronous usage, check out the
    /// BlinkEventDriven example.
    /// </para>
    /// <para>
    /// This example illustrates how to work with Treehopper boards procedurally to blink an LED. 
    /// </para>
    /// </remarks>
    class Program
    {
        static System.Timers.Timer timer;
        static void Main(string[] args)
        {
            TreehopperUSB Board = TreehopperUSB.First();        // Get a reference to the first TreehopperUSB board connected
            if(Board != null)                                   // Our reference is null if we couldn't find a board
            {
				Console.WriteLine("Found board: " + Board);
				Board.Open();								// You must explicitly open a board before communicating with it
                Board.Pin1.MakeDigitalOutput(OutputType.PushPull);
                while (Board.IsConnected)                       // repeat this block of code until we unplug the board
                {
                    Board.Pin1.ToggleOutput();                  // toggle pin 1's output
                    Thread.Sleep(1000);                         // Wait 1 second
                }
            }
            else                                                // If our reference was null, there wasn't a board attached
            {
                Console.WriteLine("No board was found when application was started");
            }

        }
    }
}
