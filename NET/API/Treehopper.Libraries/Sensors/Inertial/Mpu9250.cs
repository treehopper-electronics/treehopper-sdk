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

        /// <summary>
        /// Discover any MPU9250 IMUs attached to the specified bus.
        /// </summary>
        /// <param name="i2c">The bus to probe.</param>
        /// <param name="rate">The rate, in kHz, to use.</param>
        /// <returns>An awaitable task that completes with a list of of discovered sensors</returns>
        public static async Task<IList<Mpu9250>> ProbeAsync(I2C i2c, int rate=100)
        {
            var deviceList = new List<Mpu9250>();
            try
            {
                var dev = new SMBusDevice(0x68, i2c, rate);
                var whoAmI = await dev.ReadByteDataAsync(0x75).ConfigureAwait(false);
                if (whoAmI == 0x71)
                    deviceList.Add(new Mpu9250(i2c, false));
            } catch(Exception ex) { }

            try
            {
                var dev = new SMBusDevice(0x69, i2c, rate);
                var whoAmI = await dev.ReadByteDataAsync(0x75).ConfigureAwait(false);
                if (whoAmI == 0x71)
                    deviceList.Add(new Mpu9250(i2c, true));
            }
            catch (Exception ex) { }

            return deviceList;
        }

        public Mpu9250(I2C i2c, bool addressPin = false, int rate = 400) : base(i2c)
        {
            mag = new Ak8975(i2c);
            mag.AutoUpdateWhenPropertyRead = false;
            _registers.intPinCfg.bypassEn = 1;
            _registers.intPinCfg.latchIntEn = 1;
            Task.Run(_registers.intPinCfg.writeAsync).Wait();
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
                if (AutoUpdateWhenPropertyRead) UpdateAsync().Wait();
                return magnetometer;
            }
        }

        /// <summary>
        /// Requests a reading from the sensor and updates its data properties with the gathered values.
        /// </summary>
        /// <returns>An awaitable Task</returns>
        /// <remarks>
        /// Note that when #AutoUpdateWhenPropertyRead is `true` (which it is, by default), this method is implicitly 
        /// called when any sensor data property is read from --- there's no need to call this method unless you set
        /// AutoUpdateWhenPropertyRead to `false`.
        /// 
        /// Unless otherwise noted, this method updates all sensor data simultaneously, which can often lead to more efficient
        /// bus usage (as well as reducing USB chattiness).
        /// </remarks>
        public override async Task UpdateAsync()
        {
            await base.UpdateAsync().ConfigureAwait(false);

            if (!EnableMagnetometer) return;
            await mag.UpdateAsync().ConfigureAwait(false);
            magnetometer = mag.Magnetometer;
        }
    }
}