using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Threading.Tasks;
using Treehopper.Libraries.Sensors.Magnetic;

namespace Treehopper.Libraries.Sensors.Inertial
{
    /// <summary>
    ///     InvenSense MPU9250 9-DoF IMU
    /// </summary>
    [Supports("InvenSense", "MPU-9250")]
    public class Mpu9250 : Mpu6050, IMagnetometer
    {
        /// <summary>
        ///     The 3-axis magnetometer data
        /// </summary>
        protected Vector3 magnetometer;
        Ak8975 mag;

        public static IList<Mpu9250> Probe(I2C i2c)
        {
            var retVal = new List<Mpu9250>();
            try
            {
                var dev = new SMBusDevice(0x68, i2c, 100);
                var whoAmI = Task.Run(() => dev.ReadByteData(0x75)).Result;
                if (whoAmI == 0x71)
                    retVal.Add(new Mpu9250(i2c, false));
            } catch(Exception ex) { }

            try
            {
                var dev = new SMBusDevice(0x69, i2c, 100);
                var whoAmI = Task.Run(() => dev.ReadByteData(0x75)).Result;
                if (whoAmI == 0x71)
                    retVal.Add(new Mpu9250(i2c, true));
            }
            catch (Exception ex) { }

            return retVal;
        }

        public Mpu9250(I2C i2c, bool addressPin = false, int rate = 400) : base(i2c)
        {
            mag = new Ak8975(i2c);
            mag.AutoUpdateWhenPropertyRead = false;
            _registers.intPinCfg.bypassEn = 1;
            _registers.intPinCfg.latchIntEn = 1;
            Task.Run(_registers.intPinCfg.write).Wait();
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
            await mag.Update().ConfigureAwait(false);
            magnetometer = mag.Magnetometer;
        }
    }
}