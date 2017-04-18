using System;
using System.Threading.Tasks;
using Treehopper.Libraries.Motors;
using Treehopper.Desktop;

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
            var stepper = new Amis30624(board.I2c, false, 100);

            stepper.MinVelocityFactorThirtySeconds = 1;
            stepper.RunCurrent = Amis30624.RunningCurrent.mA_800;
            stepper.Acceleration = Amis30624.Accel.StepsPerSec2_1004;
            stepper.MaxVelocity = Amis30624.MaxVelocityType.StepsPerSecond_973;

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
