using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Magnetic
{
    /// <summary>
    /// AKM AK8975 & AK8975C magnetometer
    /// </summary>
    [Supports("AKM", "AK8975")]
    [Supports("AKM", "AK8975C")]
    public partial class Ak8975 : MagnetometerBase
    {
        SMBusDevice dev;
        private Ak8975Registers _registers;

        /// <summary>
        /// Construct a new AKM AK8975 attached to the bus
        /// </summary>
        /// <param name="i2c">The bus to probe.</param>
        /// <param name="rate">The rate, in kHz, to use.</param>
        public Ak8975(I2C i2c, int rate=100)
        {
            dev = new SMBusDevice(0x0c, i2c, rate);
            _registers = new Ak8975Registers(dev);
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
            _registers.control.mode = 1;
            Task.Run(_registers.control.write).Wait();
            while(true)
            {
                await _registers.status1.read().ConfigureAwait(false);
                if (_registers.status1.drdy == 1)
                    break;
            }

            await _registers.readRange(_registers.hx, _registers.hz).ConfigureAwait(false);
            _magnetometer.X = _registers.hx.value;
            _magnetometer.Y = _registers.hy.value;
            _magnetometer.Z = _registers.hz.value;

            RaisePropertyChanged(this);
        }
    }
}
