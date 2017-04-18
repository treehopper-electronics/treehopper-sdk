using System;
using System.Threading.Tasks;
using Treehopper.Desktop;
using Treehopper.Libraries.Sensors.Inertial;

namespace Mpu9250Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            App().Wait();
        }

        static async Task App()
        {
            var board = await ConnectionService.Instance.GetFirstDeviceAsync();
            await board.ConnectAsync();

            var imu = new Mpu6050(board.I2c);

            while(!Console.KeyAvailable)
            {
                Console.WriteLine($"{imu.Accelerometer.X:0.00}, {imu.Accelerometer.Y:0.00}, {imu.Accelerometer.Z:0.00}");
                await Task.Delay(100);
            }

            board.Disconnect();
        }
    }
}
