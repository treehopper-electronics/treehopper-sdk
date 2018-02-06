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

        public Mpu9250(I2C i2c, bool addressPin = false, int rate = 400) : base(i2c)
        {
            mag = new Ak8975(i2c);
            mag.AutoUpdateWhenPropertyRead = false;
            _registers.IntPinCfg.BypassEn = 1;
            _registers.IntPinCfg.LatchIntEn = 1;
            Task.Run(_registers.IntPinCfg.Write).Wait();
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