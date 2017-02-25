using System;
using System.Threading;
using System.Threading.Tasks;
using Treehopper.Desktop;

namespace Treehopper.Demos.AnalogRead
{
    class Program
    {
        static TreehopperUsb board;
        static void Main(string[] args)
        {
            Connect();
        }

        static async void Connect()
        {
            Console.Write("Waiting for board to be connected...");
            board = await ConnectionService.Instance.GetFirstDeviceAsync();
            Console.WriteLine("Board found:" + board);
            await board.ConnectAsync();

            await RunApp();
        }

        private async static Task RunApp()
        {
            Pin AdcPin = board.Pins[10]; // equivalent to Pin AdcPin = board[1];
            AdcPin.ReferenceLevel = AdcReferenceLevel.VREF_3V3;
            AdcPin.Mode = PinMode.AnalogInput;
            while(true)
            {
                double voltage = await AdcPin.AwaitAnalogVoltageChange();
                Console.WriteLine(String.Format("New analog voltage: {0}V", voltage));
            }
        }
    }
}
