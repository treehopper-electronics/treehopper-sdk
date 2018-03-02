using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Utilities;

namespace Treehopper.Libraries.Sensors.Inertial
{
    [Supports("STMicroelectronics", "LIS3DH")]
    public partial class Lis3dh : AccelerometerBase
    {
        private SMBusDevice dev;
        private Lis3dhRegisters registers;

        public Lis3dh(I2C i2c, bool sdo = true)
        {
            dev = new SMBusDevice((byte)(sdo ? 0x19 : 0x18), i2c);
            registers = new Lis3dhRegisters(dev);

            if (Task.Run(registers.whoAmI.read).Result.value != 0x33)
            {
                Utility.Error("Incorrect chip ID found when addressing the LIS3DH");
            }

            registers.ctrl1.xAxisEnable = 1;
            registers.ctrl1.yAxisEnable = 1;
            registers.ctrl1.zAxisEnable = 1;
            registers.ctrl1.setOutputDataRate(OutputDataRates.Hz_1);
            registers.ctrl1.lowPowerEnable = 0;
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
            await registers.readRange(registers.outX, registers.outZ).ConfigureAwait(false);
            _accelerometer.X = (float) registers.outX.value;
            _accelerometer.Y = (float)registers.outY.value;
            _accelerometer.Z = (float)registers.outZ.value;
        }
    }
}
