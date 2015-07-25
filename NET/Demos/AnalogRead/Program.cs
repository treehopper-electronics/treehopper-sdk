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
            board.Pin1.ReferenceLevel = AdcReferenceLevel.VREF_6V6;
            board.Pin1.MakeAnalogInput();
            /// One of the most powerful features of Treehopper's API is that input pins -- analog or digital -- have event-driven
            /// interfaces that allow them to update your application without requiring polling. This particular event only fires
            /// when the 10-bit ADC's value differs from the previous value.
            board.Pin1.AnalogVoltageChanged += AnalogIn_Changed;
            //board.Pin1.DigitalValueChanged += Pin1_DigitalValueChanged;
            //board.Pin7.AnalogIn.IsEnabled = true;
            while(true)
            {
              //  byte[] retVal = board.I2C.SendReceive(0x80, new byte[] { }, 1);
                //Debug.WriteLine("Device 1, channel 0: " + ltc.Read(Ltc2305Channels.Channel0));
                //Debug.WriteLine("Device 1, channel 1: " + ltc.Read(Ltc2305Channels.Channel1));
                Thread.Sleep(100);
            }
            
        }

        static void Pin1_DigitalValueChanged(Pin sender, bool value)
        {
            Console.WriteLine(sender + " input: " + value);
        }

        static void AnalogIn_Changed(Pin sender, double voltage)
        {

            Console.WriteLine("Pin 1 voltage: " + voltage);
        }
    }
}
