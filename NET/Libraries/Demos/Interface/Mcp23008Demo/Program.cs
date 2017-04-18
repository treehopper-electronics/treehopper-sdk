using System;
using System.Threading.Tasks;
using Treehopper.Desktop;
using Treehopper.Libraries.Interface.PortExpander;

namespace Mcp23008Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            App();
        }

        static async Task App()
        {
            var board = await ConnectionService.Instance.GetFirstDeviceAsync();
            await board.ConnectAsync();

            var gpio = new Mcp23008(board.I2c);

            gpio.Pins[0].DigitalValue = true;
            gpio.Pins[7].PullUpEnabled = true;
            while(!Console.KeyAvailable)
            {
                Console.WriteLine(await gpio.Pins[7].AwaitDigitalValueChange());
            }
            
        }
    }
}
