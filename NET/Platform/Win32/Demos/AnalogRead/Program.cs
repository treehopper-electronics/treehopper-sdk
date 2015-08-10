using System;
using Treehopper;
using System.Threading;
using System.Diagnostics;
namespace AnalogRead
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Waiting for board to be attached...");
            TreehopperUsb.BoardAdded += TreehopperUsb_BoardAdded;
            Thread.Sleep(-1); // wait forever for board to be attached.
        }

        private static void TreehopperUsb_BoardAdded(TreehopperUsb board)
        {
            Console.WriteLine("Board found.");
            board.Open();
            board.Pin1.ReferenceLevel = AdcReferenceLevel.VREF_3V3;
            board.Pin1.MakeAnalogInput();

            /// One of the most powerful features of Treehopper's API is that input pins -- analog or digital -- have event-driven
            /// interfaces that allow them to update your application without requiring polling. This particular event only fires
            /// when the 10-bit ADC's value differs from the previous value.
            board.Pin1.AnalogVoltageChanged += AnalogIn_Changed;

            Thread.Sleep(Timeout.Infinite); // sleep forever while we wait for events
        }

        static void AnalogIn_Changed(Pin sender, double voltage)
        {
            Console.WriteLine("Pin 1 voltage: " + voltage);
        }
    }
}
