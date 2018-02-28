﻿using System.ComponentModel;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Temperature
{
    /// <summary>
    ///     LM75 I2c temperature sensor
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This library should work with all variants of the ubiquitous LM75, manufactured by NXP, ST, Texas
    ///         Instruments, and Maxim. Note that some of these variants provide 0.5 degree Celsius resolution, while others
    ///         provide as high as 0.125 degree resolution.
    ///     </para>
    /// </remarks>
    [Supports("Maxim", "LM75")]
    [Supports("NXP", "LM75")]
    [Supports("STMicroelectronics", "LM75")]
    [Supports("Texas Instruments", "LM75")]
    public class Lm75 : TemperatureSensor
    {
        private readonly SMBusDevice dev;

        /// <summary>
        ///     Construct a new LM75 temperature sensor
        /// </summary>
        /// <param name="i2c">The i2C port to use</param>
        /// <param name="a0">the state of the A0 pin</param>
        /// <param name="a1">the state of the A1 pin</param>
        /// <param name="a2">the state of the A2 pin</param>
        public Lm75(I2C i2c, bool a0 = false, bool a1 = false, bool a2 = false)
        {
            dev = new SMBusDevice((byte) (0x48 | (a0 ? 1 : 0) | ((a1 ? 1 : 0) << 1) | ((a2 ? 1 : 0) << 2)), i2c);
        }

        public override event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Force an update of the LM75 temperature sensor
        /// </summary>
        /// <returns>An awaitable task</returns>
        public override async Task UpdateAsync()
        {
            var data = (short) await dev.ReadWordDataBE(0x00);
            Celsius = data / 32.0 / 8.0;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Celsius)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Fahrenheit)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Kelvin)));
        }
    }
}