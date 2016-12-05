using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Treehopper;
using System.Diagnostics;
using Treehopper.Libraries;
using Treehopper.Libraries.Motors.Amis30624;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            App().ConfigureAwait(false);
        }
        private static async Task App()
        {
            var board = await ConnectionService.Instance.GetFirstDeviceAsync();
            await board.ConnectAsync();
            var stepper = new Amis30624(board.I2c, Address.Hw0, 100);

            stepper.MinVelocityFactorThirtySeconds = 1;
            stepper.RunningCurrent = RunningCurrent.mA_800;
            stepper.Acceleration = Acceleration.StepsPerSec2_1004;
            stepper.MaxVelocity = MaxVelocity.StepsPerSecond_973;

            while (!Console.KeyAvailable)
            {
                await stepper.MoveAsync(5000);
                await Task.Delay(1000);
                await stepper.MoveAsync(-5000);
                await Task.Delay(1000);
            }

            board.Disconnect();
        }
    }
}
