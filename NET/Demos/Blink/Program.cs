using System;
using Treehopper;
using System.Threading;

namespace Blink
{
    class Program
    {

        static TreehopperBoard Board;
        static System.Timers.Timer timer;
        static void Main(string[] args)
        {
            /// You may be tempted to sit in a while() loop and use Thread.Sleep() for delays,
            /// but this will block the thread that's listening for board changes. Instead,
            /// use a timer running on the application's main thread to blink the LED.
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += timer_Elapsed;
            TreehopperManager manager = new TreehopperManager();
            Console.Write("Waiting for board to be connected...");
            manager.BoardAdded += manager_BoardAdded;
            manager.BoardRemoved += manager_BoardRemoved;
            Thread.Sleep(-1); // Wait here forever.
        }

        // This is called when a board is plugged into the computer.
        static void manager_BoardAdded(object sender, TreehopperBoard board)
        {
            Board = board;
            Console.WriteLine("Board found:");
            Console.WriteLine(board.Description);
            Board.Open();
            timer.Start();
        }

        static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Board.Pin1.ToggleOutput();
        }

        static void manager_BoardRemoved(TreehopperManager sender, TreehopperBoard board)
        {
            timer.Stop();
        }
    }
}
