using System;
using System.Threading.Tasks;
using Treehopper.Desktop;
using Treehopper.Firmware;

namespace FirmwareUpdate
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Treehopper Firmware Updater");
            Run().Wait();
        }

        static async Task Run()
        {
            while(true)
            {
                while (FirmwareUpdater.Boards.Count == 0)
                {
                    Console.WriteLine("Couldn't find a Treehopper board in bootloader mode. Searching for Treehoppers to reboot...");
                    var board = await ConnectionService.Instance.GetFirstDeviceAsync();
                    if (board != null)
                    {
                        Console.WriteLine("Found board. Connecting to reboot into bootloader mode");
                        if (!await board.ConnectAsync())
                            Console.WriteLine("Couldn't connect to board. Retrying in 5 seconds...");
                        else
                        board.RebootIntoBootloader();
                    }
                    Console.WriteLine("Waiting 5 seconds to reconnect...");
                    await Task.Delay(5000);
                }

                var dfu = FirmwareUpdater.Boards[0];
                dfu.ProgressChanged += Dfu_ProgressChanged;

                Console.WriteLine("Ready to load firmware. Would you like to load with the built-in default? (Y/n)");
                string response = Console.ReadLine();
                if(response.ToLower() == "y" || response.Length == 0)
                {
                    Console.WriteLine("Loading default firmware from the Treehopper assembly");
                    await dfu.LoadAsync();
                }
                else
                {
                    Console.Write("Specify the full file name to use: ");
                    response = Console.ReadLine();
                    await dfu.Load(response);
                }
                Console.WriteLine();
                Console.WriteLine("Firmware loaded! Press any key to start again, or close this window to exit.");
                Console.ReadKey();
            }
        }

        private static void Dfu_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            drawTextProgressBar(e.ProgressPercentage, 100);
        }

        private static void drawTextProgressBar(int progress, int total)
        {
            //draw empty progress bar
            Console.CursorLeft = 0;
            Console.Write("["); //start
            Console.CursorLeft = 32;
            Console.Write("]"); //end
            Console.CursorLeft = 1;
            float onechunk = 30.0f / total;

            //draw filled part
            int position = 1;
            for (int i = 0; i < onechunk * progress; i++)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw unfilled part
            for (int i = position; i <= 31; i++)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw totals
            Console.CursorLeft = 35;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(progress.ToString() + " of " + total.ToString() + "    "); //blanks at the end remove any excess
        }
    }
}
