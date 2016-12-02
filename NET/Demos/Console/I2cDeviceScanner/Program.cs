using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;

namespace I2cDeviceScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            App().Wait();
        }

        static async Task App()
        {
            Console.WriteLine("This program will scan through all i2C addresses to discover attached devices (it will try reaching each address three times until success).");
            Console.WriteLine();
            Console.WriteLine("This program may cause certain simple devices (like port expanders) to change their ouputs, so make sure you don't have anything attached to these peripherals that might be damaged.");
            Console.WriteLine();
            TreehopperUsb.Settings.ThrowExceptions = true;

            var board = await ConnectionService.Instance.GetFirstDeviceAsync();
            await board.ConnectAsync();
            Console.WriteLine("Connected to board ({0}). Do you want to proceed? Y/n", board);
            var response = Console.ReadKey(true);
            if (response.Key == ConsoleKey.Y || response.Key == ConsoleKey.Enter)
            {

                board.I2c.Enabled = true;

                for (byte i = 1; i < 127; i++)
                {
                    Console.Write("{0}: ", i);
                    for (int j = 0; j < 3; j++)
                    {
                        try
                        {
                            var result = await board.I2c.SendReceive(i, new byte[1] { 0x00 }, 0);
                            Console.Write("Device Found!");
                            break;
                        }
                        catch (I2cTransferException ex)
                        {
                            Console.Write("...", i);
                        }
                        await Task.Delay(10);
                    }
                    Console.WriteLine("");
                }

                Console.WriteLine();
                Console.WriteLine("[Press any key to exit]");
                Console.ReadKey(false);

            } else
            {
                Console.WriteLine("Aborting...");
            }
            board.Disconnect();



        }
    }
}
