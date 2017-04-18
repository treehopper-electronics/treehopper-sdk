using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Desktop;
using Treehopper.Libraries;
using Treehopper.Libraries.Displays;
using Treehopper.Libraries.Sensors.Inertial;
using Treehopper.Utilities;

namespace HMC5883Demo
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

        static Vector3 min = new Vector3();
        static Vector3 max = new Vector3();
        static async Task App()
        {
            board = await ConnectionService.Instance.GetFirstDeviceAsync();
            await board.ConnectAsync();
            Console.WriteLine("Board connected: " + board);

            var mag = new Hmc5883l(board.I2c);
            mag.Range = Hmc5883l.RangeSetting.GAIN_0_88;

            Console.WriteLine("We'll start by doing a rudimentary magnetometer calibration. " +
                "Once prompted, move the accelerometer around in all orientations to enable the software " +
                "to capture the min and max values in all directions. This process will run for 10 seconds");

            Console.WriteLine("Press any key to start calibration.");
            Console.WriteLine();
            var response = Console.ReadKey(); // wait for a key
            Console.WriteLine("Now calibrating. Move magnetometer around for 10 seconds...");

            min.X = float.MaxValue;
            min.Y = float.MaxValue;
            min.Z = float.MaxValue;
            max.X = float.MinValue;
            max.Y = float.MinValue;
            max.Z = float.MinValue;

            for (int i=0;i<100;i++)
            {
                var reading = mag.Magnetometer;
                if (reading.X < min.X)
                    min.X = reading.X;

                if (reading.Y < min.Y)
                    min.Y = reading.Y;

                if (reading.Z < min.Z)
                    min.Z = reading.Z;

                if (reading.X > max.X)
                    max.X = reading.X;

                if (reading.Y > max.Y)
                    max.Y = reading.Y;

                if (reading.Z > max.Z)
                    max.Z = reading.Z;

                await Task.Delay(100);
            }
            Console.WriteLine("Calibration done.");
            Console.WriteLine("Press any key to stop demo.");

            Vector3 offset = (max + min) / 2f;

            while (!Console.KeyAvailable)
            {
                var reading = mag.Magnetometer;

                var normalizedX = Numbers.Map(reading.X, min.X, max.X, -1, 1);
                var normalizedY = Numbers.Map(reading.Y, min.Y, max.Y, -1, 1);
                var angle = Math.Atan2(normalizedY, normalizedX);

                angle = angle * (180 / Math.PI);
                angle -= (3.0 + (10.0 / 60.0));

                Console.WriteLine($"{angle:0.00} degrees ({reading.X:0.00}, {reading.Y:0.00}, {reading.Z:0.00})");
                await Task.Delay(100);
            }
        }
    }
}
