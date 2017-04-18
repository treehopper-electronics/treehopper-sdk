using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface.PortExpander
{
    /// <summary>
    ///     PCF8574 8-bit I/O expander
    /// </summary>
    public class Pcf8574 : PortExpander
    {
        private readonly SMBusDevice dev;
        private readonly byte[] newValues;
        private readonly int numBytes;

        private readonly byte[] oldValues;

        /// <summary>
        ///     Construct a PCF-series I/O expander
        /// </summary>
        /// <param name="i2c">the I2c module to use</param>
        /// <param name="numPins">The number of pins of this expander</param>
        /// <param name="Address0">The state of the Address0 pin</param>
        /// <param name="Address1">The state of the Address1 pin</param>
        /// <param name="Address2">The state of the Address2 pin</param>
        /// <param name="baseAddress">The base address of the chip</param>
        public Pcf8574(I2c i2c, int numPins, bool Address0, bool Address1, bool Address2,
            byte baseAddress) : base(numPins)
        {
            var address = (byte) (baseAddress | (Address0 ? 1 : 0) | ((Address1 ? 1 : 0) << 1) |
                                  ((Address2 ? 1 : 0) << 2));
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

        /// <summary>
        ///     Flush the output data to the port expander
        /// </summary>
        /// <param name="force">Whether to force a full update, even if data doesn't appear to have changed.</param>
        /// <returns>An awaitable task that completes when finished</returns>
        public override async Task Flush(bool force = false)
        {
            for (var i = 0; i < Pins.Count; i++)
                // recall that we make a pin a digital input by setting it high and reading from it
                if (Pins[i].DigitalValue || Pins[i].Mode == PortExpanderPinMode.DigitalInput)
                    newValues[i / 8] |= (byte) (1 << (i % 8));
                else
                    newValues[i / 8] &= (byte) ~(1 << (i % 8));

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
            return Flush();
        }

        /// <summary>
        ///     Called when any pin's mode changes
        /// </summary>
        /// <param name="portExpanderPin">The pin whose mode has changed</param>
        protected override Task outputModeChanged(IPortExpanderPin portExpanderPin)
        {
            // we set the I/O mode by writing a 1 or 0 to the output port, so just
            // flush out the data
            return Flush();
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