using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
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
                Console.WriteLine(string.Format("{0:0.00}, {1:0.00}, {2:0.00}", imu.Accelerometer.X, imu.Accelerometer.Y, imu.Accelerometer.Z));
                await Task.Delay(100);
            }

            board.Disconnect();
        }
    }
}
