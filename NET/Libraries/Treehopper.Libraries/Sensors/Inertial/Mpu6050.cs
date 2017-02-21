using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Libraries.Sensors.Temperature;

namespace Treehopper.Libraries.Sensors.Inertial
{
    /// <summary>
    /// An MPU9150 device
    /// </summary>
    public class Mpu6050 : TemperatureSensor, IAccelerometer, IGyroscope
    {
        /// <summary>
        /// Construct an MPU9150 9-Dof IMU
        /// </summary>
        /// <param name="i2c">The I2c module this module is connected to.</param>
        /// <param name="addressPin">The address of the module</param>
        /// <param name="ratekHz">The rate, in kHz, to use with this IC</param>
        public Mpu6050(I2c i2c, bool addressPin = false, int ratekHz = 400)
        {
            this.dev = new SMBusDevice((byte)(addressPin ? 0x69 : 0x68), i2c, ratekHz);

            byte result = dev.ReadByteData((byte)Registers.WHO_AM_I).Result;

            if (result != 0x71)
                throw new Exception("Incorrect part number attached to bus");


            dev.WriteByteData((byte)Registers.PWR_MGMT_1, 0x00).Wait(); // wake up
            Task.Delay(100).Wait();
            dev.WriteByteData((byte)Registers.PWR_MGMT_1, 0x01).Wait(); // Auto select clock source to be PLL gyroscope reference if ready else
            Task.Delay(200).Wait();
            dev.WriteByteData((byte)Registers.CONFIG, 0x03).Wait();
            dev.WriteByteData((byte)Registers.SMPLRT_DIV, 0x04).Wait();

            AccelerometerScale = AccelScale.AFS_2G;
            GyroscopeScale = GyroScale.GFS_250DPS;

            var c = dev.ReadByteData((byte)Registers.ACCEL_CONFIG2).Result;
            c &= ~0x0f & 0xff;
            c |= 0x03;
            dev.WriteByteData((byte)Registers.ACCEL_CONFIG2, c).Wait();
        }

        internal Vector3 accelerometer = new Vector3();
        internal Vector3 gyroscope = new Vector3();

        internal SMBusDevice dev;

        /// <summary>
        /// Gets the accelerometer data
        /// </summary>
        public Vector3 Accelerometer
        {
            get
            {
                if (AutoUpdateWhenPropertyRead) Update().Wait();

                return accelerometer;
            }
        }

        private AccelScale ascale;

        /// <summary>
        /// Gets or sets the accelerometer scale
        /// </summary>
        public AccelScale AccelerometerScale
        {
            get { return ascale; }

            set
            {
                ascale = value;

                var c = dev.ReadByteData((byte)Registers.ACCEL_CONFIG).Result;
                c &= (~0x18) & 0xff;
                c |= (byte)((byte)ascale << 3);
                dev.WriteByteData((byte)Registers.ACCEL_CONFIG, c).Wait();
            }
        }

        private GyroScale gscale;

        /// <summary>
        /// Gets or sets the gyroscope scale
        /// </summary>
        public GyroScale GyroscopeScale
        {
            get { return gscale; }

            set
            {
                gscale = value;

                var c = dev.ReadByteData((byte)Registers.GYRO_CONFIG).Result;
                c &= (~0x02) & 0xff;
                c &= (~0x18) & 0xff;
                c |= (byte)((byte)gscale << 3);
                dev.WriteByteData((byte)Registers.GYRO_CONFIG, c).Wait();
            }
        }

        /// <summary>
        /// Gets the gyroscope data
        /// </summary>
        public Vector3 Gyroscope
        {
            get
            {
                if (AutoUpdateWhenPropertyRead) Update().Wait();

                return gyroscope;
            }
        }

        /// <summary>
        /// Read the current sensor data
        /// </summary>
        /// <returns></returns>
        public override async Task Update()
        {
            var data = await dev.ReadBufferData((byte)Registers.ACCEL_XOUT_H, 14);
            double accelScale = getAccelScale();
            double gyroScale = getGyroScale();
            accelerometer.X = (float)(((short)(data[0] << 8 | data[1])) * accelScale - accelerometerOffset.X);
            accelerometer.Y = (float)(((short)(data[2] << 8 | data[3])) * accelScale - accelerometerOffset.Y);
            accelerometer.Z = (float)(((short)(data[4] << 8 | data[5])) * accelScale - accelerometerOffset.Z);

            Celsius = (data[6] << 8 | data[7]) / 333.87 + 21.0;

            gyroscope.X = (float)(((short)(data[8] << 8 | data[9])) * gyroScale - gyroscopeOffset.X);
            gyroscope.Y = (float)(((short)(data[10] << 8 | data[11])) * gyroScale - gyroscopeOffset.Y);
            gyroscope.Z = (float)(((short)(data[12] << 8 | data[13])) * gyroScale - gyroscopeOffset.Z);
        }
        Vector3 accelerometerOffset = new Vector3();
        Vector3 gyroscopeOffset = new Vector3();

        /// <summary>
        /// Calibrate IMU relative to gravity
        /// </summary>
        /// <returns></returns>
        public virtual async Task Calibrate()
        {
            Vector3 accelOffset = new Vector3();
            Vector3 gyroOffset = new Vector3();

            accelOffset.X = 0;
            accelOffset.Y = 0;
            accelOffset.Z = 0;

            gyroOffset.X = 0;
            gyroOffset.Y = 0;
            gyroOffset.Z = 0;

            for(int i=0;i<80;i++)
            {
                await Update(); // get dater
                accelOffset.X += Accelerometer.X;
                accelOffset.Y += Accelerometer.Y;
                accelOffset.Z += Accelerometer.Z;

                gyroOffset.X += Gyroscope.X;
                gyroOffset.Y += Gyroscope.Y;
                gyroOffset.Z += Gyroscope.Z;
                await Task.Delay(10);
            }

            accelOffset.X /= 80.0f;
            accelOffset.Y /= 80.0f;
            accelOffset.Z /= 80.0f;

            // subtract off gravity
            if (accelOffset.Z > 0.5) accelOffset.Z -= 1.0f;
            else if (accelOffset.Z < -0.5) accelOffset.Z += 1.0f;

            gyroOffset.X /= 80.0f;
            gyroOffset.Y /= 80.0f;
            gyroOffset.Z /= 80.0f;

            accelerometerOffset = accelOffset;
            gyroscopeOffset = gyroOffset;

        }



        internal double getAccelScale()
        {
            switch(AccelerometerScale)
            {
                case AccelScale.AFS_2G:
                    return 2.0 / 32768.0;
                case AccelScale.AFS_4G:
                    return 4.0 / 32768.0;
                case AccelScale.AFS_8G:
                    return 8.0 / 32768.0;
                case AccelScale.AFS_16G:
                    return 16.0 / 32768.0;
                default:
                    return 0;
            }
        }

        internal double getGyroScale()
        {
            switch (GyroscopeScale)
            {
                case GyroScale.GFS_250DPS:
                    return 250.0 / 32768.0;
                case GyroScale.GFS_500DPS:
                    return 500.0 / 32768.0;
                case GyroScale.GFS_1000DPS:
                    return 1000.0 / 32768.0;
                case GyroScale.GFS_2000DPS:
                    return 2000.0 / 32768.0;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Accelerometer scales
        /// </summary>
        public enum AccelScale
        {
            /// <summary>
            /// 2G scale
            /// </summary>
            AFS_2G = 0,

            /// <summary>
            /// 4G scale
            /// </summary>
            AFS_4G,

            /// <summary>
            /// 8G scale
            /// </summary>
            AFS_8G,

            /// <summary>
            /// 16G scale
            /// </summary>
            AFS_16G
        };

        /// <summary>
        /// The gyroscope scale
        /// </summary>
        public enum GyroScale
        {
            /// <summary>
            /// 250 degrees per second
            /// </summary>
            GFS_250DPS = 0,

            /// <summary>
            /// 500 degrees per second
            /// </summary>
            GFS_500DPS,

            /// <summary>
            /// 1000 degrees per second
            /// </summary>
            GFS_1000DPS,

            /// <summary>
            /// 2000 degrees per second
            /// </summary>
            GFS_2000DPS
        };

        internal enum Registers
        {
            // Gyroscope and accelerometer
            SELF_TEST_X = 0x0D,
            SELF_TEST_Y = 0x0E,
            SELF_TEST_Z = 0x0F,
            SELF_TEST_A = 0x10,
            SMPLRT_DIV = 0x19,
            CONFIG = 0x1A,
            GYRO_CONFIG = 0x1B,
            ACCEL_CONFIG = 0x1C,
            ACCEL_CONFIG2 = 0x1D,
            FIFO_EN = 0x23,
            I2C_MST_CTRL = 0x24,

            I2C_SLV0_ADDR = 0x25,
            I2C_SLV0_REG = 0x26,
            I2C_SLV0_CTRL = 0x27,

            I2C_SLV1_ADDR = 0x28,
            I2C_SLV1_REG = 0x29,
            I2C_SLV1_CTRL = 0x2A,

            I2C_SLV2_ADDR = 0x2B,
            I2C_SLV2_REG = 0x2C,
            I2C_SLV2_CTRL = 0x2D,

            I2C_SLV3_ADDR = 0x2E,
            I2C_SLV3_REG = 0x2F,
            I2C_SLV3_CTRL = 0x30,

            I2C_SLV4_ADDR = 0x31,
            I2C_SLV4_REG = 0x32,
            I2C_SLV4_DO = 0x33,
            I2C_SLV4_CTRL = 0x34,
            I2C_SLV4_DI = 0x35,

            I2C_MST_STATUS = 0x36,
            INT_PIN_CFG = 0x37,
            INT_ENABLE = 0x38,
            INT_STATUS = 0x3A,

            ACCEL_XOUT_H = 0x3B,
            ACCEL_XOUT_L = 0x3C,
            ACCEL_YOUT_H = 0x3D,
            ACCEL_YOUT_L = 0x3E,
            ACCEL_ZOUT_H = 0x3F,
            ACCEL_ZOUT_L = 0x40,

            TEMP_OUT_H = 0x41,
            TEMP_OUT_L = 0x42,

            GYRO_XOUT_H = 0x43,
            GYRO_XOUT_L = 0x44,
            GYRO_YOUT_H = 0x45,
            GYRO_YOUT_L = 0x46,
            GYRO_ZOUT_H = 0x47,
            GYRO_ZOUT_L = 0x48,

            EXT_SENS_DATA_00 = 0x49,
            EXT_SENS_DATA_01 = 0x4A,
            EXT_SENS_DATA_02 = 0x4B,
            EXT_SENS_DATA_03 = 0x4C,
            EXT_SENS_DATA_04 = 0x4D,
            EXT_SENS_DATA_05 = 0x4E,
            EXT_SENS_DATA_06 = 0x4F,
            EXT_SENS_DATA_07 = 0x50,
            EXT_SENS_DATA_08 = 0x51,
            EXT_SENS_DATA_09 = 0x52,
            EXT_SENS_DATA_10 = 0x53,
            EXT_SENS_DATA_11 = 0x54,
            EXT_SENS_DATA_12 = 0x55,
            EXT_SENS_DATA_13 = 0x56,
            EXT_SENS_DATA_14 = 0x57,
            EXT_SENS_DATA_15 = 0x58,
            EXT_SENS_DATA_16 = 0x59,
            EXT_SENS_DATA_17 = 0x5A,
            EXT_SENS_DATA_18 = 0x5B,
            EXT_SENS_DATA_19 = 0x5C,
            EXT_SENS_DATA_20 = 0x5D,
            EXT_SENS_DATA_21 = 0x5E,
            EXT_SENS_DATA_22 = 0x5F,
            EXT_SENS_DATA_23 = 0x60,

            I2C_SLV0_D0 = 0x63,
            I2C_SLV1_DO = 0x64,
            I2C_SLV2_DO = 0x65,
            I2C_SLV3_D0 = 0x66,
            I2C_MST_DELAY_CTRL = 0x67,
            SIGNAL_PATH_RESET = 0x68,
            USER_CTRL = 0x6A,
            PWR_MGMT_1 = 0x6B,
            PWR_MGMT_2 = 0x6C,
            FIFO_COUNTH = 0x72,
            FIFO_COUNTL = 0x73,
            FIFO_R_W = 0x74,
            WHO_AM_I = 0x75,
        }


    }
}
