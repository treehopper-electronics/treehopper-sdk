using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Desktop;
using Treehopper.Libraries;
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
            ConnectionService.Instance.Boards.CollectionChanged += Boards_CollectionChanged;
        }

        private static async void Boards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var board = (TreehopperUsb)e.NewItems[0];
            await board.ConnectAsync();

            //board.Pins[0].Mode = PinMode.PushPullOutput;

            while (board.IsConnected && !Console.KeyAvailable)
            {
                // do stuff
                board.Led = !board.Led;
                await Task.Delay(100);
            }
        }

        static async Task App()
        {
            //var board = await ConnectionService.Instance.GetFirstDeviceAsync();
            

        }
    }
}
