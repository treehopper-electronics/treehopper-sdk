using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Inertial
{
    /// <summary>
    /// ST L3GD20H / L3GD20 / L3G4200D gyroscope
    /// </summary>
    /// <remarks>
    /// This driver supports three different ST gyroscopes: the L3GD20H, L3GD20, and L3G4200D.
    /// The newer L3GD20H and L3GD20 have a 0x6A/0x6B address; the older L3G4200D has a 0x68/0x69 address.
    /// There are two constructors: one for the L3GD20 and L3GD20H (with selectable SDO addressing pin), 
    /// and one that takes a raw address value that can be used with any of these sensors with any SDO pin configuration.
    /// 
    /// We recommend using the Probe() static method to discover the correct address configuration for the device attached
    /// to your board.
    /// </remarks>
    [Supports("ST", "L3GD20")]
    [Supports("ST", "L3GD20H")]
    [Supports("ST", "L3G4200D")]
    public partial class L3gd20 : GyroscopeBase
    {
        L3gd20Registers registers;

        /// <summary>
        /// Discover any L3GD20H, L3GD20, or L3G4200D gyroscopes attached to this system.
        /// </summary>
        /// <param name="i2c">The bus to scan.</param>
        /// <param name="rate">The rate, in kHz, to use.</param>
        /// <returns></returns>
        public static async Task<IList<L3gd20>> ProbeAsync(I2C i2c, int rate=100)
        {
            var devList = new List<L3gd20>();

            try
            {
                // L3G4200D - SDO low
                var dev = new SMBusDevice(0x68, i2c, 100);
                var whoAmI = await dev.ReadByteDataAsync(0x0f).ConfigureAwait(false);
                if (whoAmI == 0xD3)
                    devList.Add(new L3gd20(i2c, 0x68, rate));
            }
            catch (Exception ex) { }

            try
            {
                // L3G4200D - SDO high
                var dev = new SMBusDevice(0x69, i2c, 100);
                var whoAmI = await dev.ReadByteDataAsync(0x0f).ConfigureAwait(false);
                if (whoAmI == 0xD3)
                    devList.Add(new L3gd20(i2c, 0x69, rate));
            }
            catch (Exception ex) { }

            try
            {
                // L3GD20 - SDO low
                var dev = new SMBusDevice(0x6A, i2c, 100);
                var whoAmI = await dev.ReadByteDataAsync(0x0f).ConfigureAwait(false);
                if (whoAmI == 0xD4)
                    devList.Add(new L3gd20(i2c, 0x6A, rate));
            }
            catch (Exception ex) { }

            try
            {
                // L3GD20 - SDO high
                var dev = new SMBusDevice(0x6B, i2c, 100);
                var whoAmI = await dev.ReadByteDataAsync(0x0f).ConfigureAwait(false);
                if (whoAmI == 0xD4)
                    devList.Add(new L3gd20(i2c, 0x6B, rate));
            }
            catch (Exception ex) { }

            try
            {
                // L3GD20H - SDO low
                var dev = new SMBusDevice(0x6A, i2c, 100);
                var whoAmI = await dev.ReadByteDataAsync(0x0f).ConfigureAwait(false);
                if (whoAmI == 0xD7)
                    devList.Add(new L3gd20(i2c, 0x6A, rate));
            }
            catch (Exception ex) { }

            try
            {
                // L3GD20H - SDO high
                var dev = new SMBusDevice(0x6B, i2c, 100);
                var whoAmI = await dev.ReadByteDataAsync(0x0f).ConfigureAwait(false);
                if (whoAmI == 0xD7)
                    devList.Add(new L3gd20(i2c, 0x6B, rate));
            }
            catch (Exception ex) { }

            return devList;
        }

        /// <summary>
        /// Construct a new L3GD20H, L3GD20, or L3G4200D with the specified address.
        /// </summary>
        /// <param name="i2c">The bus to use</param>
        /// <param name="address">The address of the L3GD20H, L3GD20, or L3G4200D</param>
        /// <param name="rate">The rate to use.</param>
        public L3gd20(I2C i2c, byte address, int rate = 100)
        {
            registers = new L3gd20Registers(new SMBusDevice(address, i2c, rate));
            Task.Run(registers.ctrlReg1.write).Wait();
            registers.ctrlReg1.xEn = 1;
            registers.ctrlReg1.yEn = 1;
            registers.ctrlReg1.zEn = 1;
            registers.ctrlReg1.pd = 1;
            registers.ctrlReg1.setDataRate(DataRates.Hz_190);
            Task.Run(registers.ctrlReg1.write).Wait();
            Task.Run(registers.ctrlReg2.write).Wait();
            Task.Run(registers.ctrlReg5.write).Wait();
        }

        /// <summary>
        /// Construct a new L3GD20 or L3GD20H
        /// </summary>
        /// <param name="i2c">The bus to use.</param>
        /// <param name="sdoPin">The address pin (SA0/SDO) state.</param>
        /// <param name="rate">The rate to use.</param>
        public L3gd20(I2C i2c, bool sdoPin = false, int rate = 100) : this(i2c, (byte)(sdoPin ? 0x6A : 0x6B), rate)
        {
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
            await registers.readRange(registers.outX, registers.outZ);
            gyroscope.X = registers.outX.value * 0.00875f;
            gyroscope.Y = registers.outY.value * 0.00875f;
            gyroscope.Z = registers.outZ.value * 0.00875f;
        }
    }
}
