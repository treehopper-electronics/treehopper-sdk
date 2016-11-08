using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Treehopper;
using Treehopper.Libraries.Amis30624;
using System.Diagnostics;
using Treehopper.Libraries;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            App().ConfigureAwait(false);
        }
        static SevenSegSpi display;
        private static async Task App()
        {
            var board = await ConnectionService.Instance.GetFirstDeviceAsync();
            await board.ConnectAsync();
            board.Spi.ChipSelect = board.Pins[5];
            display = new SevenSegSpi(board.Spi);
            var stepper = new Amis30624(board.I2c, Address.Hw0, 100);

            stepper.MinVelocityFactorThirtySeconds = 1;
            stepper.RunningCurrent = RunningCurrent.mA_800;
            stepper.Acceleration = Acceleration.StepsPerSec2_1004;
            stepper.MaxVelocity = MaxVelocity.StepsPerSecond_973;

            stepper.PositionChanged += Stepper_PositionChanged;

            while (!Console.KeyAvailable)
            {
                await stepper.MoveAsync(5000);
                await Task.Delay(1000);
                await stepper.MoveAsync(-5000);
                await Task.Delay(1000);
            }

            board.Disconnect();
        }

        private static void Stepper_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            display.printNumber(e.Position).Wait();
        }
    }
}
