using System.Threading.Tasks;
using Treehopper.Desktop;
using Treehopper.Libraries.Displays;

namespace Ht16k33Demo
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

            var driver = new Ht16k33(board.I2c, Ht16k33.Package.Sop20_64Led);
            for(int i=0;i<driver.Leds.Count;i++)
            {
                driver.Leds[i].State = true;
                await Task.Delay(40);
            }

            for (int i = 0; i < driver.Leds.Count; i++)
            {
                driver.Leds[i].State = false;
                await Task.Delay(40);
            }

            board.Disconnect();
        }
    }
}
