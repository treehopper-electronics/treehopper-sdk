using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface.PortExpander
{
    /// <summary>
    /// An 8-bit I2c I/O expander
    /// </summary>
    public class Pcf8574 : PortExpander
    {
        private SMBusDevice dev;
        int numBytes;

        /// <summary>
        /// Construct a PCF-series I/O expander
        /// </summary>
        /// <param name="i2c">the I2c module to use</param>
        /// <param name="numPins">The number of pins of this expander</param>
        /// <param name="Address0">The state of the Address0 pin</param>
        /// <param name="Address1">The state of the Address1 pin</param>
        /// <param name="Address2">The state of the Address2 pin</param>
        /// <param name="baseAddress">The base address of the chip</param>
        public Pcf8574(I2c i2c, int numPins, bool Address0, bool Address1, bool Address2, byte baseAddress) : base(numPins)
        {
            byte address = (byte)(baseAddress | (Address0 ? 1 : 0) | (Address1 ? 1 : 0) << 1 | (Address2 ? 1 : 0) << 2);
            dev = new SMBusDevice(address, i2c);

            numBytes = numPins / 8;

            oldValues = new byte[numBytes];
            newValues = new byte[numBytes];

            // make all pins inputs by default
            AutoFlush = false;
            foreach (var pin in Pins)
                pin.Mode = PortExpanderPinMode.DigitalInput;
            AutoFlush = true;
            Flush(true).Wait();

        }

        byte[] oldValues;
        byte[] newValues;

        /// <summary>
        /// Flush the output data to the port expander
        /// </summary>
        /// <param name="force">Whether to force a full update, even if data doesn't appear to have changed.</param>
        /// <returns>An awaitable task that completes when finished</returns>
        public override async Task Flush(bool force = false)
        {
            for (int i = 0; i < Pins.Count; i++)
            {
                // recall that we make a pin a digital input by setting it high and reading from it
                if (Pins[i].DigitalValue == true || Pins[i].Mode == PortExpanderPinMode.DigitalInput)
                    newValues[i / 8] |= (byte)(1 << (i % 8));
                else
                    newValues[i / 8] &= (byte)~(1 << (i % 8));
            }

            bool shouldResend = false;

            for (int i = 0; i < oldValues.Length; i++)
            {
                if (oldValues[i] != newValues[i])
                    shouldResend = true;
            }

            if (shouldResend || force)
            {
                newValues.CopyTo(oldValues, 0);

                await dev.WriteData(newValues);
            }

        }
        /// <summary>
        /// Called when the output value of any pin has changed and should be written to the port
        /// </summary>
        /// <param name="portExpanderPin">The pin whose value changed</param>
        protected override void outputValueChanged(IPortExpanderPin portExpanderPin)
        {
            Flush().Wait();
        }

        /// <summary>
        /// Called when any pin's mode changes
        /// </summary>
        /// <param name="portExpanderPin">The pin whose mode has changed</param>
        protected override void outputModeChanged(IPortExpanderPin portExpanderPin)
        {
            // we set the I/O mode by writing a 1 or 0 to the output port, so just
            // flush out the data
            Flush().Wait();
        }

        /// <summary>
        /// Read the port's current inputs and update the values accordingly
        /// </summary>
        /// <returns>An awaitable task that completes when finished</returns>
        protected override async Task readPort()
        {
            var data = await dev.ReadData((byte)numBytes);
            for (int i = 0; i < Pins.Count; i++)
            {
                int bank = i / 8;
                int bit = i % 8;
                if (data[bank] >> bit == 0x01)
                    Pins[i].UpdateInputValue(true);
                else
                    Pins[i].UpdateInputValue(false);
            }
        }
    }
}
