using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Utilities;
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
            RxApp();
        }

        static async Task RxApp()
        {
            var board = await ConnectionService.Instance.GetFirstDeviceAsync();
            await board.ConnectAsync();

            var rf = new Nrf24l01(board.Spi, board.Pins[3], board.Pins[4], board.Pins[5]);
            rf.AddressWidth = 3;
            rf.TxAddress = 0xC2C2C2;
            rf.Pipes[0].Address = 0xC2C2C2;
            rf.Pipes[0].EnablePipe = true;
            rf.Pipes[1].Address = 0xC2C2C3;
            rf.Pipes[1].EnablePipe = true;
            rf.Pipes[2].Address = 0xC2C2C4;
            rf.Pipes[2].EnablePipe = true;
            rf.Pipes[3].Address = 0xC2C2C5;
            rf.Pipes[3].EnablePipe = true;
            rf.RetryCount = 5;
            rf.RetryDelayUs = 1500;
            rf.Channel = 9;
            rf.Rate = Nrf24l01.DataRate.Rate_250kbps;
            rf.RxEnable = true;

            rf.Pipes[1].DataReceived += Program_DataReceived1;
            rf.Pipes[2].DataReceived += Program_DataReceived2;
            rf.Pipes[3].DataReceived += Program_DataReceived3;


            while (true)
            {
                for (int i = 0; i < 255; i++)
                {
                    await Task.Delay(100);
                }
            }
        }

        private static void Program_DataReceived1(object sender, Nrf24l01.DataReceivedEventArgs e)
        {
            Console.WriteLine("Pipe #1: {0}", e.DataReceived.ToHexString());
        }

        private static void Program_DataReceived2(object sender, Nrf24l01.DataReceivedEventArgs e)
        {
            Console.WriteLine("Pipe #2: {0}", e.DataReceived.ToHexString());
        }

        private static void Program_DataReceived3(object sender, Nrf24l01.DataReceivedEventArgs e)
        {
            Console.WriteLine("Pipe #3: {0}", e.DataReceived.ToHexString());
        }

        static async Task TxApp()
        {
			var board = await ConnectionService.Instance.GetFirstDeviceAsync();
			await board.ConnectAsync();

            var rf = new Nrf24l01(board.Spi, board.Pins[3], board.Pins[4], board.Pins[5]);
            rf.TxAddress = 0xC2C2C2;
            rf.Pipes[0].Address = 0xC2C2C2;
            rf.AddressWidth = 3;
            rf.RetryCount = 5;
            rf.RetryDelayUs = 1500;
            rf.Channel = 9;
            rf.Rate = Nrf24l01.DataRate.Rate_250kbps;


            while(true)
            {
                for(int i=0;i<255;i++)
                {
                    await rf.Transmit(new byte[] { (byte)i, 0x00, 0x23, 0x37, 0x37, 0x13 }, false);
                    await Task.Delay(100);
                }
                    
            }
            
        }

    }
}
