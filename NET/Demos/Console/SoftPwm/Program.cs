using System;
using System.Threading.Tasks;
using Treehopper.Desktop;

namespace Treehopper.Demos.SoftPwm
{
    class Program
    {
        static void Main(string[] args)
        {
            Run();
        }
        static TreehopperUsb board;
        static async void Run()
        {
            Console.Write("Looking for board...");
            board = await ConnectionService.Instance.GetFirstDeviceAsync();
            Console.WriteLine("Board found.");
            await board.ConnectAsync();
            var pin = board[19];

            pin.Mode = PinMode.SoftPwm;
            pin.DutyCycle = 0.8;
            int step = 10;
            int rate = 25;
            while (true)
            {
                for (int i = 0; i < 256; i = i + step)
                {
                    pin.DutyCycle = i / 255.0;
                    await Task.Delay(rate);
                }
                for (int i = 255; i > 0; i = i - step)
                {
                    pin.DutyCycle = i / 255.0;
                    await Task.Delay(rate);
                }
            }
        }
    }
}
