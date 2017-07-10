﻿using System.ComponentModel;
using System.Numerics;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Inertial
{
    /// <summary>
    ///     InvenSense MPU9250 9-DoF IMU
    /// </summary>
    [Supports("InvenSense", "MPU-9250")]
    public class Mpu9250 : Mpu6050, IMagnetometer
    {
        private readonly SMBusDevice mag;

        /// <summary>
        ///     The 3-axis magnetometer data
        /// </summary>
        protected Vector3 magnetometer;


        private Mpu9250(SMBusDevice dev, SMBusDevice mag) : base(dev)
        {
            this.mag = mag;
            this.dev = dev;
        }

        /// <summary>
        ///     Construct a new MPU9250 9-DoF IMU
        /// </summary>
        /// <param name="i2c">The i2C port to use</param>
        /// <param name="addressPin">The address pin state</param>
        /// <param name="rate">The rate, in kHz, to communicate at</param>
        public static async Task<Mpu9250> Create(I2C i2c, bool addressPin = false, int rate = 400)
        {
            var result = await Mpu6050.Create(i2c, addressPin, rate).ConfigureAwait(false);
            var dev = result.dev;
            await dev.WriteByteData((byte)Registers.INT_PIN_CFG, 0x22).ConfigureAwait(false);
            var mag = new SMBusDevice(0x0C, i2c, rate);

            return new Mpu9250(dev, mag);
        }

        /// <summary>
        ///     Whether the magnetometer is enabled or not
        /// </summary>
        public bool EnableMagnetometer { get; set; } = true;

        /// <summary>
        ///     The current 3-axis magnetometer data
        /// </summary>
        public Vector3 Magnetometer
        {
            get
            {
                if (AutoUpdateWhenPropertyRead) Update().Wait();
                return magnetometer;
            }
        }

        /// <summary>
        ///     Retrieve the latest sample data from the MPU9250
        /// </summary>
        /// <returns></returns>
        public override async Task Update()
        {
            await base.Update().ConfigureAwait(false);

            if (!EnableMagnetometer) return;

            var magData = await mag.ReadBufferData((byte) AK8975CRegisters.HXL, 6).ConfigureAwait(false);

            magnetometer.X = (magData[1] << 8) | magData[0];
            magnetometer.Y = (magData[3] << 8) | magData[2];
            magnetometer.Z = (magData[5] << 8) | magData[4];
        }

        /// <summary>
        ///     Registers for the AK8975C magnetometer
        /// </summary>
        private enum AK8975CRegisters
        {
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
            ASAZ = 0x12
        }
    }
}