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
    public partial class Lsm303dlhcAccel : IAccelerometer
    {
        Lsm303dlhcAccelRegisters registers;
        Vector3 _accelerometer;
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
        /// Gets a vector with the acceleration data, in g.
        /// </summary>
        /// <remarks>
        /// \f$1\, \text{g} = 9.8\, \text{m}/\text{s}^2\f$
        /// </remarks>
        public Vector3 Accelerometer
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    Task.Run(UpdateAsync).Wait();

                return _accelerometer;
            }
        }

        public bool AutoUpdateWhenPropertyRead { get; set; } = true;

        public async Task UpdateAsync()
        {
            await registers.readRange(registers.outAccelX, registers.outAccelZ).ConfigureAwait(false);
            _accelerometer.X = registers.outAccelX.value / 16f * 0.001f;
            _accelerometer.Y = registers.outAccelY.value / 16f * 0.001f;
            _accelerometer.Z = registers.outAccelZ.value / 16f * 0.001f;
        }
    }
}
