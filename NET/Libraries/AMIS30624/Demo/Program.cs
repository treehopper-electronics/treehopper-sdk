using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Treehopper;
using Treehopper.Libraries.Amis30624;

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

            var stepper = new Amis30624(board.I2c, Address.Hw0);
            stepper.RunningCurrent = RunningCurrent.mA_800;
            stepper.HoldingCurrent = HoldingCurrent.mA_673;
            stepper.MaxVelocity = MaxVelocity.StepsPerSecond_973;
            await stepper.RunVelocity();
            //await Task.Delay(5000);
            //stepper.ShaftDirection = false;
            //await Task.Delay(5000);

            await stepper.SoftStop();
        }
    }
}
