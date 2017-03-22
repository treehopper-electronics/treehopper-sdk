﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;

namespace Treehopper.Libraries.Interface.PortExpander
{
    /// <summary>
    /// 595-type 8-bit parallel-output shift register. 
    /// </summary>
    public class Hc595 : ShiftOut
    {
        /// <summary>
        /// Construct a new 74HC595-type shift register that is directly connected to a Treehopper's SPI port
        /// </summary>
        /// <remarks>
        /// This class supports all 74HC595 shift registers. The name of the class comes from the widely-available TI part.
        /// </remarks>
        public Hc595(Spi spiModule, SpiChipSelectPin latchPin, double speedMhz = 1) : base(spiModule, latchPin, 8, SpiMode.Mode00, ChipSelectMode.PulseHighAtEnd, speedMhz)
        {
        }

        /// <summary>
        /// Construct a 595-type shift register connected to the output of another 595-type shift register.
        /// </summary>
        /// <param name="UpstreamDevice"></param>
        public Hc595(ShiftOut UpstreamDevice) : base(UpstreamDevice, 8)
        {
        }
    }
}
