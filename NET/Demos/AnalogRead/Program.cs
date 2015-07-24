using System;
using Treehopper;
using System.Threading;
using System.Diagnostics;
using Treehopper.Libraries;
namespace AnalogRead
{
    class Program
    {
        static void Main(string[] args)
        {
            TreehopperManager manager = new TreehopperManager();
            Console.Write("Waiting for board to be attached...");
            manager.BoardAdded += manager_BoardAdded;
            Thread.Sleep(-1); // wait forever for board to be attached.
        }

        static void manager_BoardAdded(TreehopperManager sender, TreehopperUSB board)
        {
            Console.WriteLine("Board found.");
            board.Open();

            /// One of the most powerful features of Treehopper's API is that input pins -- analog or digital -- have event-driven
            /// interfaces that allow them to update your application without requiring polling. This particular event only fires
            /// when the 10-bit ADC's value differs from the previous value.
            //board.Pin7.AnalogIn.VoltageChanged += AnalogIn_Changed;

            //board.Pin7.AnalogIn.IsEnabled = true;
            Ltc2305 ltc = new Treehopper.Libraries.Ltc2305(0x8, board.I2C);
            Ltc2305 ltc2 = new Treehopper.Libraries.Ltc2305(0x19, board.I2C);
            while(true)
            {
              //  byte[] retVal = board.I2C.SendReceive(0x80, new byte[] { }, 1);
                //Debug.WriteLine("Device 1, channel 0: " + ltc.Read(Ltc2305Channels.Channel0));
                //Debug.WriteLine("Device 1, channel 1: " + ltc.Read(Ltc2305Channels.Channel1));
                Debug.WriteLine("Device 2, channel 0: " + ltc2.Read(Ltc2305Channels.Channel0));
                Debug.WriteLine("Device 2, channel 1: " + ltc2.Read(Ltc2305Channels.Channel1));
                Thread.Sleep(100);
            }
            
        }

        static void AnalogIn_Changed(Pin sender, double voltage)
        {
            Console.WriteLine("Pin 7 voltage: " + voltage);
        }
    }
}
