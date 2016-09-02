using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            int pinNumber = 10;
            Console.Write("Looking for board...");
            board = await ConnectionService.Instance.First();
            Console.WriteLine("Board found.");
            Console.WriteLine(String.Format("Connecting to {0} and starting SoftPwm on Pin{1}", board, pinNumber));
            await board.Connect();
            board[pinNumber].SoftPwm.Enabled = true;
            int step = 10;
            int rate = 25;
            while (true)
            {
                for (int i = 0; i < 256; i = i + step)
                {
                    board[pinNumber].SoftPwm.DutyCycle = i / 255.0;
                    await Task.Delay(rate);
                }
                for (int i = 255; i > 0; i = i - step)
                {
                    board[pinNumber].SoftPwm.DutyCycle = i / 255.0;
                    await Task.Delay(rate);
                }
            }
        }
    }
}
