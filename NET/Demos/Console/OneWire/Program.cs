using System;
using Treehopper;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;

namespace Blink
{
    /// <summary>
    /// This demo blinks an LED attached to pin 1, using basic procedural programming.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Treehopper provides an asynchronous, event-driven API that helps your apps respond to plug / unplug detection.
    /// While this is the recommended way of working with TreehopperUsb references, many users new to C# and high-level languages
    /// in general may be uncomfortable with event-driven programming. To learn more about asynchronous usage, check out the
    /// BlinkEventDriven example.
    /// </para>
    /// <para>
    /// This example illustrates how to work with Treehopper boards procedurally to blink an LED. 
    /// </para>
    /// </remarks>
    class Program
    {
        static void Main(string[] args)
        {
            RunBlink();
        }

        static async void RunBlink()
        {
            while (true)
            {
                Console.Write("Waiting for board...");
                // Get a reference to the first TreehopperUsb board connected. This will await indefinitely until a board is connected.
                TreehopperUsb Board = await ConnectionService.Instance.GetFirstDeviceAsync();
                Console.WriteLine("Found board: " + Board);

                // You must explicitly open a board before communicating with it
                await Board.ConnectAsync();

                Board.Uart.Mode = UartMode.OneWire;
                Board.Uart.Enabled = true;



                //OneWire ow = new OneWire(Board.Uart);
                List<UInt64> addresses = await Board.Uart.OneWireSearch();

                while (true)
                {
                    bool result = await Board.Uart.OneWireReset();
                    //await Board.Uart.Send(0x33);
                    //byte[] data = await Board.Uart.Receive(8);

                    //Array.Reverse(data);

                    //UInt64 addr = BitConverter.ToUInt64(data, 0);

                    //result = await Board.Uart.OneWireReset();


                    await Board.Uart.Send(new byte[] { 0xCC, 0x44 });
                    await Task.Delay(750);

                    //await Board.Uart.OneWireResetAndMatchAddress(addr);
                    ////await Board.Uart.OneWireReset();
                    ////await Board.Uart.Send(new byte[] {0xCC, 0xBE });
                    //await Board.Uart.Send(0xBE);

                    //data = await Board.Uart.Receive(2);

                    //await Board.Uart.OneWireReset();
                    //double temp = ((Int16)(data[0] | (data[1] << 8))) / 16.0;
                    //Console.WriteLine(String.Format("Temperature from {0}: {1} F", addr, (temp * 1.8 + 32)));

                    foreach (UInt64 address in addresses)
                    {
                        await Board.Uart.OneWireResetAndMatchAddress(address);
                        await Board.Uart.Send(0xBE);

                        byte[] data = await Board.Uart.Receive(2);

                        await Board.Uart.OneWireReset();
                        double temp = ((Int16)(data[0] | (data[1] << 8))) / 16.0;
                        Console.WriteLine(String.Format("Temperature from {0}: {1} F", address, (temp * 1.8 + 32)));

                    }
                }


                // We arrive here when the board has been disconnected
                Console.WriteLine("Board has been disconnected.");
            }

        }
    }
}
