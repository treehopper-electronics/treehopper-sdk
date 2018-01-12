using System.Diagnostics;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries.IO.PortExpander;

namespace DemoSn74hc166
{
    class Program
    {
        static void Main(string[] args)
        {
            App().Wait();
        }

        static async Task App()
        {
            var board = await ConnectionService.Instance.GetFirstDeviceAsync();
            await board.ConnectAsync();
            var shiftRegister = new Hc166(board.Spi, board.Pins[10]);
            while(true)
            {
                var value = shiftRegister.Pins[0].AwaitDigitalValueChange();
                Debug.WriteLine(value);
            }
        }
    }
}
