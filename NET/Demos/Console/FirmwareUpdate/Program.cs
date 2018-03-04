using System;
using System.Threading.Tasks;
using Treehopper;
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
                Console.WriteLine("Press P to program, and T to test");
                var response = Console.ReadLine();

                if (response.ToLower() == "t")
                {
                    var board = await ConnectionService.Instance.GetFirstDeviceAsync();
                    await SelfTestAsync(board).ConfigureAwait(false);
                } else
                {
                    while (FirmwareConnectionService.Instance.Boards.Count == 0)
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

                    var dfu = FirmwareConnectionService.Instance.Boards[0];
                    dfu.ProgressChanged += Dfu_ProgressChanged;

                    Console.WriteLine("Ready to load firmware. Would you like to load with the built-in default? (Y/n)");
                    response = Console.ReadLine();
                    if (response.ToLower() == "y" || response.Length == 0)
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
        }

        private static async Task<bool> SelfTestAsync(TreehopperUsb board)
        {
            Console.WriteLine($"Beginning self-test of {board}");
            bool retVal = true;
            await board.ConnectAsync().ConfigureAwait(false);
            // make each pin an input
            foreach (var pin in board.Pins)
            {
                await pin.MakeAnalogInAsync().ConfigureAwait(false);
                await board.AwaitPinUpdateAsync().ConfigureAwait(false);
                await Task.Delay(10).ConfigureAwait(false);

                if (pin.AnalogValue < 0.1)
                {
                    retVal = false;
                    Console.WriteLine($"{pin.Name} shorted to ground.");
                }
            }


            for (int i = 0; i < board.Pins.Count - 1; i++)
            {
                board.Pins[i].Mode = PinMode.PushPullOutput;
                await board.Pins[i].WriteDigitalValueAsync(false).ConfigureAwait(false);
                await Task.Delay(10).ConfigureAwait(false);
                await board.AwaitPinUpdateAsync().ConfigureAwait(false);
                if (board.Pins[i + 1].AnalogValue < 0.1)
                {
                    Console.WriteLine($"Short detected on pins {i} and {i + 1}.");
                    retVal = false;
                }
                await board.Pins[i].WriteDigitalValueAsync(true).ConfigureAwait(false);
            }
            if (retVal == false)
            {
                Console.WriteLine("Errors found during self-test!");
            }
            else
            {
                Console.WriteLine("Self-test passed!");
            }

            return retVal;
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
