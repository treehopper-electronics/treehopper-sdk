using System;
using System.Threading.Tasks;
using Treehopper;
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

            Mpu9250 imu;
            while(true)
            {
                var imuList = await Mpu9250.ProbeAsync(board.I2c); // find the first MPU9250 attached to the bus
                if (imuList.Count == 0)
                {
                    Console.WriteLine("No MPU9250 attached (Are you sure you're not using an MPU6050?). Press any key to try again.");
                    Console.ReadKey();
                } else
                {
                    imu = imuList[0];
                    break;
                }
            }
            
            imu.AutoUpdateWhenPropertyRead = false;
            while(!Console.KeyAvailable)
            {
                await imu.UpdateAsync().ConfigureAwait(false);
                Console.WriteLine($"Temperature: {imu.Celsius:0.00} °C ({imu.Fahrenheit:0.00} °F)");
                Console.WriteLine($"IMU: {imu.Accelerometer.X:0.00}, {imu.Accelerometer.Y:0.00}, {imu.Accelerometer.Z:0.00}");
                Console.WriteLine($"Gyro: {imu.Gyroscope.X:0.00}, {imu.Gyroscope.Y:0.00}, {imu.Gyroscope.Z:0.00}");
                Console.WriteLine($"Magnetometer: {imu.Magnetometer.X:0.00}, {imu.Magnetometer.Y:0.00}, {imu.Magnetometer.Z:0.00}");
                await Task.Delay(500);
            }

            board.Disconnect();
        }
    }
}
