using System;
using Treehopper;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;

namespace OneWireScan
{
    /// <summary>
    /// Scans the OneWire bus for attached devices, and prints their 64-bit addresses.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            App().Wait();
        }

        static async Task App()
        {
            Console.Write("Waiting for board...");
            // Get a reference to the first TreehopperUsb board connected. This will await indefinitely until a board is connected.
            TreehopperUsb Board = await ConnectionService.Instance.GetFirstDeviceAsync();
            Console.WriteLine("Found board: " + Board);

            // You must explicitly open a board before communicating with it
            await Board.ConnectAsync();

            Board.Uart.Mode = UartMode.OneWire;
            Board.Uart.Enabled = true;

            List<UInt64> addresses = await Board.Uart.OneWireSearch();

            Console.WriteLine("Found addresses: ");
            foreach (var address in addresses)
                Console.WriteLine(address);

            Board.Disconnect();
        }

    }
}
