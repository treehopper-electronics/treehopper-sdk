using System;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Desktop;

namespace Sandbox
{
    /// <summary>
    /// This is the in-tree "sandbox" app that developers use to quickly test stuff out
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
			App().Wait();
        }


        static async Task App()
        {
			var board = await ConnectionService.Instance.GetFirstDeviceAsync();

			await board.ConnectAsync();

			//board.Pins[0].Mode = PinMode.PushPullOutput;

            foreach(var pin in board.Pins)
            {
                pin.Mode = PinMode.OpenDrainOutput;
                pin.DigitalValue = true;
            }

			while (board.IsConnected && !Console.KeyAvailable)
			{
                foreach (var pin in board.Pins)
                {
                    pin.DigitalValue = false;
                    await Task.Delay(500);
                    pin.DigitalValue = true;
                    await Task.Delay(500);
                }
            }

        }
    }
}
