using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Treehopper.Libraries.Utilities;

namespace Treehopper.Libraries.IO.PortExpander
{
    /// <summary>
    ///     Microchip MCP23008 8-bit I/O expander with I2c interface
    /// </summary>
    [Supports("Microchip", "MCP23008")]
    public class Mcp23008 : IPortExpander<Mcp23008.Pin>, IPortExpanderParent, IPolledEvents
    {
        private readonly SMBusDevice dev;
        private readonly BitArray gppu = new BitArray(8);
        private readonly BitArray iodir = new BitArray(8);
        private readonly BitArray olat = new BitArray(8);
        private BitArray gpio = new BitArray(8);

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Construct a new MCP23008
        /// </summary>
        /// <param name="i2c">The I2c bus to use</param>
        /// <param name="a0">The state of the A0 pin</param>
        /// <param name="a1">The state of the A1 pin</param>
        /// <param name="a2">The state of the A2 pin</param>
        /// <param name="speedKHz"></param>
        public Mcp23008(I2C i2c, bool a0 = false, bool a1 = false, bool a2 = false, int speedKHz = 100)
            : this(i2c, (byte) (0x20 | (a0 ? 1 : 0) | ((a1 ? 1 : 0) << 1) | ((a2 ? 1 : 0) << 2)), speedKHz)
        {
        }

        /// <summary>
        ///     Construct a new MCP23008
        /// </summary>
        /// <param name="i2c">The I2c bus to use</param>
        /// <param name="address">The address to use</param>
        /// <param name="speedKHz">The speed, in KHz, to use</param>
        public Mcp23008(I2C i2c, byte address, int speedKHz = 100)
        {
            dev = new SMBusDevice(address, i2c, speedKHz);
            for (var i = 0; i < 8; i++)
                Pins.Add(new Pin(this, i));
        }

        /// <summary>
        ///     Gets a list of pins associated with this GPIO expander
        /// </summary>
        public IList<Pin> Pins { get; protected set; } = new List<Pin>();

        /// <summary>
        ///     Whether or not setting a pin's value will flush out to the port immediately
        /// </summary>
        public bool AutoFlush { get; set; } = true;

        /// <summary>
        ///     The parent device. Unused and always returns null.
        /// </summary>
        public IFlushable Parent => null;

        /// <summary>
        ///     Flush out the data to port
        /// </summary>
        /// <param name="force">Whether the data should be flushed, even if it appears to be unchanged</param>
        /// <returns>An awaitable task</returns>
        public Task FlushAsync(bool force = false)
        {
            return dev.WriteByteDataAsync((byte) Registers.OutputLatch, olat.GetBytes()[0]);
        }

        /// <summary>
        ///     Gets or sets whether this GPIO expander will execute an (expensive) i2C read request whenever the DigitalValue
        ///     property is read from
        /// </summary>
        public bool AutoUpdateWhenPropertyRead { get; set; } = true;

        /// <summary>
        ///     Gets or sets the value used during pin's AwaitDigitalValueChanged method if AutoUpdateWhenPropertyRead is true
        /// </summary>
        public int AwaitPollingInterval { get; set; } = 25;

        async Task IPortExpanderParent.OutputValueChanged(IPortExpanderPin portExpanderPin)
        {
            olat.Set(portExpanderPin.PinNumber, portExpanderPin.DigitalValue);
            if (AutoFlush)
                await FlushAsync().ConfigureAwait(false);
        }

        async Task IPortExpanderParent.OutputModeChanged(IPortExpanderPin portExpanderPin)
        {
            var pinNumber = portExpanderPin.PinNumber;
            iodir.Set(pinNumber, portExpanderPin.Mode == PortExpanderPinMode.DigitalInput);
            await dev.WriteByteDataAsync((byte) Registers.IoDirection, iodir.GetBytes()[0]).ConfigureAwait(false);

            var pin = Pins[pinNumber];
            gppu.Set(pinNumber, pin.PullUpEnabled);
            await dev.WriteByteDataAsync((byte) Registers.GpPullUp, gppu.GetBytes()[0]).ConfigureAwait(false);
        }

        /// <summary>
        /// Requests an update from the port and update its data properties with the gathered values.
        /// </summary>
        /// <returns>An awaitable Task</returns>
        /// <remarks>
        /// Note that when #AutoUpdateWhenPropertyRead is `true` (which it is, by default), this method is implicitly 
        /// called when any sensor data property is read from --- there's no need to call this method unless you set
        /// AutoUpdateWhenPropertyRead to `false`.
        /// 
        /// Unless otherwise noted, this method updates all sensor data simultaneously, which can often lead to more efficient
        /// bus usage (as well as reducing USB chattiness).
        /// </remarks>
        public Task UpdateAsync()
        {
            return readPort();
        }

        /// <summary>
        ///     Read the current values of the port and update the pins
        /// </summary>
        /// <returns></returns>
        protected async Task readPort()
        {
            var data = await dev.ReadByteDataAsync((byte) Registers.Gpio).ConfigureAwait(false);
            gpio = new BitArray(new[] {data});
            for (var i = 0; i < gpio.Length; i++)
                Pins[i].UpdateInputValue(gpio.Get(i));
        }

        private enum Registers
        {
            IoDirection,
            InputPolarity,
            GpIntEnable,
            DefaultValue,
            InteruptConfig,
            IoConfig,
            GpPullUp,
            IntFlag,
            IntCap,
            Gpio,
            OutputLatch
        }

        /// <summary>
        ///     An MCP23008 pin, including the PullUpEnabled functionality
        /// </summary>
        public class Pin : PortExpanderPin
        {
            private bool pullUp;

            internal Pin(Mcp23008 portExpander, int pinNumber) : base(portExpander, pinNumber)
            {
            }

            /// <summary>
            ///     Whether the pin's internal Pull-Up resistor is enabled or not
            /// </summary>
            public bool PullUpEnabled
            {
                get { return pullUp; }

                set
                {
                    if (pullUp == value) return;
                    pullUp = value;
                    portExpander.OutputModeChanged(this);
                }
            }
        }
    }
}