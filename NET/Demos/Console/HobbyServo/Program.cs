using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;

namespace HobbyServo
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
            board = await ConnectionService.Instance.First();
            Console.WriteLine("Board found:" + board);
            await board.Connect();

            await RunApp();
        }

        static async Task RunApp()
        {
            Pin servoPin = board.Pin11; // equivalent to Pin servoPin = board[11];
            int stepSize = 25;
            int rate = 1;

            Console.WriteLine("Connected. Starting demo.");
            servoPin.SoftPwm.Enabled = true;

            while (true)
            {
                for (int i = 0; i < 2050; i = i + stepSize)
                {
                    servoPin.SoftPwm.PulseWidth = 800 + i;
                    await Task.Delay(rate);
                }
                for (int i = 2050; i > 0; i = i - stepSize)
                {
                    servoPin.SoftPwm.PulseWidth = 800 + i;
                    await Task.Delay(rate);
                }
            }
        }




        async void Start()
        {
            TreehopperUsb board = await ConnectionServiceWin.Instance.First();
            await board.Connect();
            board.Pin1.Mode = PinMode.PushPullOutput;
            while(true)
            {
                board.Pin1.ToggleOutput();
                await Task.Delay(1000);
            }
        }










        async void Stop()
        {
            TreehopperUsb board = await ConnectionServiceWin.Instance.First();
            await board.Connect();
            board.Pin1.Mode = PinMode.PushPullOutput;
            while (true)
            {
                board.Pin1.ToggleOutput();
                await Task.Delay(1000);
            }



        }






    }
}
