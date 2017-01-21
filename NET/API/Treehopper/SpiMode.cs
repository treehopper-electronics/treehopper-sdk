using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    /// <summary>
    /// Defines the clock phase and polarity used for SPI communication
    /// </summary>
    /// <remarks>
    /// <para>The left number indicates the clock polarity (CPOL), while the right number indicates the clock phase (CPHA). Consult https://en.wikipedia.org/wiki/Serial_Peripheral_Interface_Bus#Clock_polarity_and_phase for more information.</para>
    /// <para>Note that the numeric values of this enum do not match the standard nomenclature, but instead match the value needed by Treehopper's MCU. Do not attempt to cast integers from/to this enum.</para>
    /// </remarks>
    public enum SpiMode
    {
        /// <summary>
        /// Clock is initially low; data is valid on the rising edge of the clock
        /// </summary>
        Mode00 = 0x00,

        /// <summary>
        /// Clock is initially low; data is valid on the falling edge of the clock
        /// </summary>
        Mode01 = 0x20,

        /// <summary>
        /// Clock is initially high; data is valid on the rising edge of the clock
        /// </summary>
        Mode10 = 0x10,

        /// <summary>
        /// Clock is initially high; data is valid on the falling edge of the clock
        /// </summary>
        Mode11 = 0x30
    };
}
