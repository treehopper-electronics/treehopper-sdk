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
            App();
            Thread.Sleep(-1);
        }

        static async Task App()
        {
            Console.Write("Waiting for board to be connected...");
            board = await ConnectionService.Instance.GetFirstDeviceAsync();
            Console.WriteLine("Board found:" + board);
            await board.ConnectAsync();

            Pin AdcPin = board.Pins[0]; // equivalent to Pin AdcPin = board[1];
            AdcPin.ReferenceLevel = AdcReferenceLevel.Vref_3V3;
            AdcPin.Mode = PinMode.AnalogInput;
            while (!Console.KeyAvailable)
            {
                double voltage = await AdcPin.AwaitAnalogVoltageChange();
                Console.WriteLine($"New analog voltage: {voltage}V");
            }
        }
    }
}
