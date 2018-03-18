using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Inertial
{
    /// <summary>
    /// Accelerometer portion of the LSM303DLHC IMU
    /// </summary>
    public partial class Lsm303dlhcAccel : AccelerometerBase
    {
        Lsm303dlhcAccelRegisters registers;

        public Lsm303dlhcAccel(I2C i2c, int rate=100)
        {
            registers = new Lsm303dlhcAccelRegisters(new SMBusDevice(0x19, i2c, rate));
            registers.ctrl1.setOutputDataRate(OutputDataRates.Hz_100);
            registers.ctrl1.xEnable = 1;
            registers.ctrl1.yEnable = 1;
            registers.ctrl1.zEnable = 1;
            Task.Run(registers.ctrl1.write).Wait();
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
            await registers.readRange(registers.outAccelX, registers.outAccelZ).ConfigureAwait(false);
            _accelerometer.X = registers.outAccelX.value / 16f * 0.001f;
            _accelerometer.Y = registers.outAccelY.value / 16f * 0.001f;
            _accelerometer.Z = registers.outAccelZ.value / 16f * 0.001f;
        }
    }
}
