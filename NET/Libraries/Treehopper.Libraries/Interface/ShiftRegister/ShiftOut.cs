using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface.ShiftRegister
{
    public class ShiftOut : IFlushableOutputPort<ShiftOutPin>
    {
        public ShiftOut(ShiftOut upstreamDevice, uint numPins = 8)
        {
            setup(numPins);

        }

        public ShiftOut(Spi spiModule, Pin latchPin, uint numPins = 8, SPIMode mode = SPIMode.Mode00, ChipSelectMode csMode = ChipSelectMode.PulseHighAtEnd, double speedMhz = 1)
        {
            setupSpi(spiModule, latchPin, speedMhz, mode, csMode);
            setup(numPins);
        }

        private void setupSpi(Spi spiModule, Pin latchPin, double speedMhz, SPIMode mode, ChipSelectMode csMode)
        {
            spiDevice = new SpiDevice(spiModule, latchPin, speedMhz, mode, csMode);
        }

        private static SpiDevice spiDevice;

        private static Collection<ShiftOut> shiftRegisters = new Collection<ShiftOut>();


        private static Task requestWrite()
        {
            // build the byte array to flush out of the port
            List<byte> bytes = new List<byte>();
            var reversedList = shiftRegisters.Reverse(); // we push out the last device's data first
            foreach (var shift in reversedList)
            {
                var shiftBytes = BitConverter.GetBytes(shift.CurrentValue);

                // who knows, maybe we're on ARM?
                if (!BitConverter.IsLittleEndian)
                    shiftBytes = shiftBytes.Reverse().ToArray();

                // We'll always end up with 4 bytes, but we can't send more bytes than our shift register is expecting (8, 16, etc)
                for(int i=0;i<shift.Pins.Count / 8;i++)
                {
                    bytes.Add(shiftBytes[i]);
                }
            }

            return spiDevice.SendReceive(bytes.ToArray());
        }

        private void setup(uint numPins)
        {
            if (numPins > 32)
                throw new Exception("This library only supports shift registers up to 32-bit wide");

            for (int i = 0; i < numPins; i++)
                Pins.Add(new ShiftOutPin(this, i));

            shiftRegisters.Add(this);
            ShiftOut.requestWrite().Wait();
        }
        public bool AutoFlush { get; set; } = true;
        public uint CurrentValue { get; protected set; }
        uint lastValues;

        /// <summary>
        /// Immediately update all the pins at once with the given value
        /// </summary>
        /// <param name="value">A value representing the data to write to the port</param>
        /// <returns>An awaitable task that completes upon successfully writing the value.</returns>
        public async Task Write(uint value)
        {
            bool savedAutoFlushValue = AutoFlush;
            AutoFlush = false;

            for(int i=0;i<Pins.Count;i++)
            {
                Pins[i].DigitalValue = ((value >> i) & 1) == 0x01 ? true : false;
            }

            await Flush();
            AutoFlush = savedAutoFlushValue;
        }

        public Collection<ShiftOutPin> Pins { get; protected set; } = new Collection<ShiftOutPin>();

        public async Task Flush(bool force = false)
        {
            if(CurrentValue != lastValues || force)
            {
                await ShiftOut.requestWrite();
                lastValues = CurrentValue;
            }
        }

        internal void UpdateOutput(ShiftOutPin shiftOutPin)
        {
            if (shiftOutPin.DigitalValue)
                CurrentValue |= (uint)(1 << shiftOutPin.PinNumber);
            else
                CurrentValue &= (uint)~(1 << shiftOutPin.PinNumber);

            if (AutoFlush)
                Flush().Wait();
        }
    }
}
