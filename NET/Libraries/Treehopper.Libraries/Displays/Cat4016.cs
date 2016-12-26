using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Libraries.Interface.ShiftRegister;

namespace Treehopper.Libraries.Displays
{
    public class Cat4016 : Stp16cpc26
    {
        /// <summary>
        /// Construct an Cat4016 attached directly to a board SPI module
        /// </summary>
        /// <param name="SpiModule">The board's SPI module</param>
        /// <param name="LatchPin">The pin to use for latches</param>
        /// <param name="BlankPin">The digital pin, if any, to control the display state (via the Blank input)</param>
        public Cat4016(Spi SpiModule, Pin LatchPin, DigitalOutPin BlankPin = null) : base(SpiModule, LatchPin, BlankPin)
        {

        }

        /// <summary>
        /// Construct an Cat4016 attached directly to a board SPI module
        /// </summary>
        /// <param name="SpiModule">The board's SPI module</param>
        /// <param name="LatchPin">The pin to use for latches</param>
        /// <param name="BlankPin">The PWM pin to use, allowing controllable global brightness (via the Blank input)</param>
        public Cat4016(Spi SpiModule, Pin LatchPin, Pwm BlankPin) : base(SpiModule, LatchPin, BlankPin)
        {
        }

        /// <summary>
        /// Construct an Cat4016 attached to the output of another shift register
        /// </summary>
        /// <param name="upstreamDevice">The upstream device this shift register is attached to</param>
        /// <param name="BlankPin">The digital pin to use, if any, to control the display state (via the Blank input)</param>
        public Cat4016(ChainableShiftRegisterOutput upstreamDevice, DigitalOutPin BlankPin = null) : base(upstreamDevice, BlankPin)
        {

        }

        /// <summary>
        /// Construct an Cat4016 attached to the output of another shift register
        /// </summary>
        /// <param name="upstreamDevice">The upstream device this shift register is attached to</param>
        /// <param name="BlankPin">The PWM pin to use, if any, to control the display brightness (via the Blank input)</param>
        public Cat4016(ChainableShiftRegisterOutput upstreamDevice, Pwm BlankPin) : base(upstreamDevice, BlankPin)
        {

        }
    }
}
