using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;

namespace Treehopper.Libraries.Interface.ShiftRegister
{
    /// <summary>
    /// 595-type 8-bit parallel-output shift register. 
    /// </summary>
    public class Sn74hc595 : ShiftOut
    {
        /// <summary>
        /// Construct a new 595-type shift register that is directly connected to a Treehopper's SPI port
        /// </summary>
        public Sn74hc595(Spi spiModule, Pin latchPin, double speedMhz = 1) : base(spiModule, latchPin, 8, SPIMode.Mode00, ChipSelectMode.PulseHighAtEnd, speedMhz)
        {

        }

        /// <summary>
        /// Construct a 595-type shift register connected to the output of another 595-type shift register.
        /// </summary>
        /// <param name="UpstreamDevice"></param>
        public Sn74hc595(ShiftOut UpstreamDevice) : base(UpstreamDevice, 8)
        {

        }
    }
}
