using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Inertial
{
    /// <summary>
    /// Analog Devices ADXL345 three-axis accelerometer
    /// </summary>
    [Supports("Analog Devices", "ADXL-345")]
    public partial class Adxl345 : AccelerometerBase
    {
        private readonly SMBusDevice _dev;
        private readonly Adxl345Registers registers;

        public static async Task<IList<Adxl345>> Probe(I2C i2c, int rate=100)
        {
            List<Adxl345> deviceList = new List<Adxl345>();

            try
            {
                var dev = new SMBusDevice(0x53, i2c, 100);
                var whoAmI = await dev.ReadByteData(0x00).ConfigureAwait(false);
                if (whoAmI == 0xE5)
                    deviceList.Add(new Adxl345(i2c, true, rate));
            }
            catch (Exception ex) { }

            try
            {
                var dev = new SMBusDevice(0x1D, i2c, 100);
                var whoAmI = await dev.ReadByteData(0x00).ConfigureAwait(false);
                if (whoAmI == 0xE5)
                    deviceList.Add(new Adxl345(i2c, false, rate));
            }
            catch (Exception ex) { }

            return deviceList;
        }

        /// <summary>
        /// Construct a new Analog Devices ADXL345.
        /// </summary>
        /// <param name="i2c">The I2C bus to use.</param>
        /// <param name="altAddressPin">The state of the address pin.</param>
        /// <param name="rate">The rate, in kHz, to use.</param>
        public Adxl345(I2C i2c, bool altAddressPin = false, int rate = 100)
        {
            _dev = new SMBusDevice((byte)(!altAddressPin ? 0x1D : 0x53), i2c, rate);
            registers = new Adxl345Registers(_dev);
            registers.powerCtl.sleep = 0;
            registers.powerCtl.measure = 1;
            registers.dataFormat.range = 0x00;
            registers.dataFormat.fullRes = 1;
            Task.Run(() => registers.writeRange(registers.powerCtl, registers.dataFormat)).Wait();
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
            await registers.readRange(registers.dataX, registers.dataZ).ConfigureAwait(false);
            _accelerometer.X = registers.dataX.value * 0.004f;
            _accelerometer.Y = registers.dataY.value * 0.004f;
            _accelerometer.Z = registers.dataZ.value * 0.004f;
        }
    }
}
