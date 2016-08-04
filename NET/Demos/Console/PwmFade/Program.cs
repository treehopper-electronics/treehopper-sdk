using System;
using Treehopper;
using System.Threading;
using System.Threading.Tasks;

namespace PwmFade
{
	class Program
	{
		static void Main(string[] args)
		{
            RunApp();
		}

        static async void RunApp()
		{
            Console.Write("Waiting for board to be connected...");
            TreehopperUsb board = await ConnectionService.Instance.First();
			Console.WriteLine("Board found:" + board);
            await board.Connect();
            board.Pwm1.IsEnabled = true;
            board.PwmManager.Frequency = PwmFrequency.Freq_61Hz; 

            int step = 5;
            int rate = 1;
            while (true)
            {
                for (int i = 0; i < 256; i = i + step)
                {
                    board.Pwm1.DutyCycle = i / 255.0;
                    await Task.Delay(rate);
                }
                for (int i = 255; i > 0; i = i - step)
                {
                    board.Pwm1.DutyCycle = i / 255.0; 
                    await Task.Delay(rate);
                }
            }
        }
	}
}
