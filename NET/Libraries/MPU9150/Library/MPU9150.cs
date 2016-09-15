using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Inertial.MPU9150
{
    public class MPU9150 : SMBusDevice
    {
        IDigitalPin intPin;
        public MPU9150(II2c i2c, MPU9160Address address) : base((byte)address, i2c)
        {
            WriteByteData((byte)Registers.PWR_MGMT_1, 0x00).Wait();
            Update();
        }

        private async void Update()
        {
            while(true)
            {
                var accel = new Vector()
                {
                    X = (double)(await ReadWordData((byte)Registers.ACCEL_XOUT_H)) / ushort.MaxValue,
                    Y = (double)(await ReadWordData((byte)Registers.ACCEL_YOUT_H)) / ushort.MaxValue,
                    Z = (double)(await ReadWordData((byte)Registers.ACCEL_ZOUT_H)) / ushort.MaxValue
                };
                Accel = accel;

                var gyro = new Vector()
                {
                    X = (double)(await ReadWordData((byte)Registers.GYRO_XOUT_H)) / ushort.MaxValue,
                    Y = (double)(await ReadWordData((byte)Registers.GYRO_YOUT_H)) / ushort.MaxValue,
                    Z = (double)(await ReadWordData((byte)Registers.GYRO_ZOUT_H)) / ushort.MaxValue
                };
                Gyro = gyro;
                await Task.Delay(1);
            }
        }

        public Vector Accel { get; set; }
        public Vector Gyro { get; set; }

        enum Registers
        {
            // Magnetometer (AK8975C)
            WIA = 0x00,
            INFO = 0x01,
            ST1 = 0x02,
            HXL = 0x03,
            HXH = 0x04,
            HYL = 0x05,
            HYH = 0x06,
            HZL = 0x07,
            HZH = 0x08,

            ST2 = 0x09,
            CNTL = 0x0A,
            RSV = 0x0B,
            ASTC = 0x0C,
            TS1 = 0x0D,
            TS2 = 0x0E,
            I2CDIS = 0x0F,
            ASAX = 0x10,
            ASAY = 0x11,
            ASAZ = 0x12,

            // Gyroscope and accelerometer
            SELF_TEST_X = 0x0D,
            SELF_TEST_Y = 0x0E,
            SELF_TEST_Z = 0x0F,
            SELF_TEST_A = 0x10,
            SMPLRT_DIV = 0x19,
            CONFIG = 0x1A,
            GYRO_CONFIG = 0x1B,
            ACCEL_CONFIG = 0x1C,
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

    public enum MPU9160Address
    {
        AD0_LOW = 0x68,
        AD0_HIGH = 0x69
    }

    public struct Vector
    {
        public double X;
        public double Y;
        public double Z;
    }
}
