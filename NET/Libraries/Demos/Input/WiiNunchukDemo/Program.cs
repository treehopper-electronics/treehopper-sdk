using System;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Desktop;
using Treehopper.Libraries.Input;
using Treehopper.Libraries.Sensors;

namespace WiiNunchukDemo
{
    class Program
    {
        static TreehopperUsb board;
        static void Main(string[] args)
        {
            App().Wait();
            board.Disconnect();
            Console.WriteLine("Board disconnected");
        }

        static async Task App()
        {
            board = await ConnectionService.Instance.GetFirstDeviceAsync();
            await board.ConnectAsync();

            var nunchuk = new WiiNunchuk(board.I2c);

            // Let a Poller take care of the updating
            using (var poller = new Poller<WiiNunchuk>(nunchuk))
            {
                // Hook onto some fun events
                nunchuk.JoystickChanged += Nunchuk_JoystickChanged;
                nunchuk.C.OnPressed += C_OnPressed;
                nunchuk.Z.OnPressed += Z_OnPressed;

                Console.WriteLine("Starting demo. Press any key to stop...");

                while (board.IsConnected && !Console.KeyAvailable)
                {
                    await Task.Delay(100);
                }
            }
        }

        private static void C_OnPressed(object sender, Button.ButtonPressedEventArgs e)
        {
            // the implicity-called ToString() method on the button object (sender, in this context) will print "pressed" or "not pressed" -- handy for console/debug logging!
            Console.WriteLine("Button C: " + sender);
        }

        private static void Z_OnPressed(object sender, Button.ButtonPressedEventArgs e)
        {
            Console.WriteLine("Button Z: " + sender);
        }


        private static void Nunchuk_JoystickChanged(object sender, WiiNunchuk.JoystickEventArgs e)
        {
            Console.WriteLine("Joystick: {0:0.00}, {1:0.00}", e.NewValue.X, e.NewValue.Y);
        }
    }
}
