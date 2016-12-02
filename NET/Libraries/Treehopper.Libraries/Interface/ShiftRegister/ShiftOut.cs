using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface.ShiftRegister
{
    public class ShiftOut : ChainableShiftRegisterOutput, IFlushableOutputPort<ShiftOutPin>
    {
        public ShiftOut(ChainableShiftRegisterOutput upstreamDevice, int numPins = 8) : base(upstreamDevice, numPins/8)
        {
            setup(numPins);

        }

        public ShiftOut(Spi spiModule, Pin latchPin, int numPins = 8, SPIMode mode = SPIMode.Mode00, ChipSelectMode csMode = ChipSelectMode.PulseHighAtEnd, double speedMhz = 1)
            : base(spiModule, latchPin, numPins/8, mode, csMode, speedMhz)
        {
            setup(numPins);
        }


        private void setup(int numPins)
        {
            if (numPins > 32)
                throw new Exception("This library only supports shift registers up to 32-bit wide");

            for (int i = 0; i < numPins; i++)
                Pins.Add(new ShiftOutPin(this, i));

            ChainableShiftRegisterOutput.requestWrite().Wait();
        }

        public Collection<ShiftOutPin> Pins { get; protected set; } = new Collection<ShiftOutPin>();


        internal void UpdateOutput(ShiftOutPin shiftOutPin)
        {
            if (shiftOutPin.DigitalValue)
                CurrentValue |= (uint)(1 << shiftOutPin.BitNumber);
            else
                CurrentValue &= (uint)~(1 << shiftOutPin.BitNumber);

            FlushIfAutoFlushEnabled().Wait();
        }

        protected override void updateFromCurrentValue()
        {
            uint currentValue = CurrentValue; // CurrentValue is an expensive read, so only read it once
            for (int i = 0; i < Pins.Count; i++)
            {
                Pins[i].DigitalValue = ((currentValue >> i) & 1) == 0x01 ? true : false;
            }
        }
    }
}
