using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Treehopper.Libraries.Utilities;

namespace Treehopper.Libraries.IO.PortExpander
{
    /// <summary>
    ///     Represents standard <see cref="DigitalOut" />-equipped shift registers
    /// </summary>
    public class ShiftOut : ChainableShiftRegisterOutput, IFlushableOutputPort<ShiftOutPin>
    {
        private BitArray currentValue;

        /// <summary>
        ///     Construct a shift register attached to an upstream device
        /// </summary>
        /// <param name="upstreamDevice">The upstream device this shift register is attached to</param>
        /// <param name="numPins">The number of pins this shift register has</param>
        public ShiftOut(ChainableShiftRegisterOutput upstreamDevice, int numPins = 8) : base(upstreamDevice,
            numPins / 8)
        {
            setup(numPins);
        }

        /// <summary>
        ///     Construct a shift register attached to an SPI port
        /// </summary>
        /// <param name="spiModule">The SPI module this shift register is attached to</param>
        /// <param name="latchPin">The latch pin to use</param>
        /// <param name="numPins">The number of pins on the shift register</param>
        /// <param name="mode">THe SPI mode to use with this shift register (and subsequent ones on this chain)</param>
        /// <param name="csMode">The chip select mode to use with this shift register (and subsequent ones on this chain)</param>
        /// <param name="speedMhz">The speed to operate this shift register (and subsequent ones on this chain) with</param>
        public ShiftOut(Spi spiModule, SpiChipSelectPin latchPin, int numPins = 8, SpiMode mode = SpiMode.Mode00,
            ChipSelectMode csMode = ChipSelectMode.PulseHighAtEnd, double speedMhz = 1)
            : base(spiModule, latchPin, numPins / 8, speedMhz, mode, csMode)
        {
            setup(numPins);
        }

        /// <summary>
        ///     Collection of pins for this shift register
        /// </summary>
        public IList<ShiftOutPin> Pins { get; protected set; } = new Collection<ShiftOutPin>();


        private void setup(int numPins)
        {
            currentValue = new BitArray(numPins);

            if (numPins > 32)
                throw new Exception("This library only supports shift registers up to 32-bit wide");

            for (var i = 0; i < numPins; i++)
                Pins.Add(new ShiftOutPin(this, i));

            Task.Run(() => Flush()).Wait();
        }

        internal void UpdateOutput(ShiftOutPin shiftOutPin)
        {
            currentValue.Set(shiftOutPin.BitNumber, shiftOutPin.DigitalValue);
            CurrentValue = currentValue.GetBytes();

            FlushIfAutoFlushEnabled().Wait();
        }

        /// <summary>
        ///     Update the Pins structure from the current value
        /// </summary>
        protected override void updateFromCurrentValue()
        {
            currentValue = new BitArray(CurrentValue);
            for (var i = 0; i < Pins.Count; i++)
                Pins[i].DigitalValue = currentValue.Get(i);
        }
    }
}