using System.Diagnostics;
using System.Threading.Tasks;

namespace Treehopper.Libraries.IO.PortExpander
{
    /// <summary>
    ///     PCF8574 8-bit I/O expander
    /// </summary>
    [Supports("Texas Instruments", "PCF8574")]
    [Supports("NXP", "PCF8574")]
    public class Pcf8574 : PortExpander
    {
        private readonly SMBusDevice dev;
        private byte[] newValues;
        private readonly int numBytes;

        private byte[] oldValues;

        /// <summary>
        ///     Construct a PCF-series I/O expander
        /// </summary>
        /// <param name="i2c">the I2c module to use</param>
        /// <param name="Address0">The state of the Address0 pin</param>
        /// <param name="Address1">The state of the Address1 pin</param>
        /// <param name="Address2">The state of the Address2 pin</param>
        /// <param name="baseAddress">The base address of the chip</param>
        /// <param name="numPins">The number of pins of this expander</param>
        public Pcf8574(I2C i2c, bool Address0 = false, bool Address1 = false, bool Address2 = false,
            byte baseAddress = 0x20, int numPins = 8) : base(numPins)
        {
            var address = (byte) (baseAddress | (Address0 ? 1 : 0) | ((Address1 ? 1 : 0) << 1) |
                                  ((Address2 ? 1 : 0) << 2));
            dev = new SMBusDevice(address, i2c);

            numBytes = numPins / 8;

            oldValues = new byte[numBytes];

            // make all pins inputs by default
            AutoFlush = false;
            foreach (var pin in Pins)
                pin.Mode = PortExpanderPinMode.DigitalInput;
            AutoFlush = true;
            Task.Run(() => Flush(true)).Wait();
        }

        /// <summary>
        ///     Flush the output data to the port expander
        /// </summary>
        /// <param name="force">Whether to force a full update, even if data doesn't appear to have changed.</param>
        /// <returns>An awaitable task that completes when finished</returns>
        public override async Task Flush(bool force = false)
        {
            newValues = new byte[numBytes];
            for (var i = 0; i < Pins.Count; i++)
            {
                // recall that we make a pin a digital input by setting it high and reading from it
                if (Pins[i].DigitalValue || Pins[i].Mode == PortExpanderPinMode.DigitalInput)
                    newValues[i / 8] |= (byte)(1 << (i % 8));
                else
                    newValues[i / 8] &= (byte)~(1 << (i % 8));
            }

            var shouldResend = false;

            for (var i = 0; i < oldValues.Length; i++)
                if (oldValues[i] != newValues[i])
                    shouldResend = true;

            if (shouldResend || force)
            {
                newValues.CopyTo(oldValues, 0);

                await dev.WriteData(newValues);
            }
        }

        /// <summary>
        ///     Called when the output value of any pin has changed and should be written to the port
        /// </summary>
        /// <param name="portExpanderPin">The pin whose value changed</param>
        protected override Task outputValueChanged(IPortExpanderPin portExpanderPin)
        {
            if(AutoFlush)
                return Flush();
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Called when any pin's mode changes
        /// </summary>
        /// <param name="portExpanderPin">The pin whose mode has changed</param>
        protected override Task outputModeChanged(IPortExpanderPin portExpanderPin)
        {
            // we set the I/O mode by writing a 1 or 0 to the output port, so just
            // flush out the data
            if (AutoFlush)
                return Flush();
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Read the port's current inputs and update the values accordingly
        /// </summary>
        /// <returns>An awaitable task that completes when finished</returns>
        protected override async Task readPort()
        {
            var data = await dev.ReadData((byte) numBytes);
            for (var i = 0; i < Pins.Count; i++)
            {
                var bank = i / 8;
                var bit = i % 8;
                if (data[bank] >> bit == 0x01)
                    Pins[i].UpdateInputValue(true);
                else
                    Pins[i].UpdateInputValue(false);
            }
        }
    }
}