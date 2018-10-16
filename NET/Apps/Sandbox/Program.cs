using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Desktop;
using Treehopper.Libraries.IO.Adc;
using Treehopper.Libraries.IO.DigitalPot;
using Treehopper.Libraries.Sensors.Optical;
using Treehopper.Libraries.Sensors.Pressure;
using Treehopper.Libraries.Sensors.Temperature;
using Treehopper.Libraries.Wireless;

namespace Sandbox
{
    /// <summary>
    /// This is the in-tree "sandbox" app that developers use to quickly test stuff out
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            App();
        }

        static async Task App()
        {
			var board = await ConnectionService.Instance.GetFirstDeviceAsync();
			await board.ConnectAsync();

            var rf = new Nrf24l01(board.Spi, board.Pins[3], board.Pins[4], board.Pins[5]);
            var address = BitConverter.ToInt32(Encoding.ASCII.GetBytes("2Node\0"), 0);

            while(true)
            {
                await rf.Transmit(address, new byte[] { 0x00, 0x01, 0x02 }, false);
                await Task.Delay(1000);
            }
            
        }

    }
}
