using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Treehopper.ThirdParty;

namespace Treehopper.Libraries.IO.PortExpander
{
    /// <summary>
    ///     Any shift-register-like device that can be daisy-chained onto other shift registers.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Note that this class doesn't expose a collection of <see cref="DigitalOut" />s  or <see cref="Displays.Led" />
    ///         s, and instead, represents any writable shift register device. See <see cref="ShiftOut" /> for an
    ///         implementation of a pin-based shift register.
    ///     </para>
    /// </remarks>
    public abstract class ChainableShiftRegisterOutput : IFlushable
    {
        private readonly AsyncLock lockObject = new AsyncLock();

        private byte[] lastValues;
        private SpiDevice spiDevice;
        private int numBytes;

        public IList<ChainableShiftRegisterOutput> Children { get; set; } = new List<ChainableShiftRegisterOutput>();

        /// <summary>
        ///     Set up a ChainableShiftRegisterOutput connected to an SPI port.
        /// </summary>
        /// <param name="spiModule">the SPI module to use</param>
        /// <param name="latchPin">The latch pin to use</param>
        /// <param name="numBytes">The number of bytes to write to this device</param>
        /// <param name="mode">The SPI mode to use for all shift registers in this chain</param>
        /// <param name="csMode">The ChipSelectMode to use for all shift registers in this chain</param>
        /// <param name="speedMhz">The speed to use for all shift registers in this chain</param>
        public ChainableShiftRegisterOutput(Spi spiModule, SpiChipSelectPin latchPin, int numBytes = 1,
            double speedMhz = 6, SpiMode mode = SpiMode.Mode00, ChipSelectMode csMode = ChipSelectMode.PulseHighAtEnd)
        {
            spiDevice = new SpiDevice(spiModule, latchPin, csMode, speedMhz, mode);
            this.numBytes = numBytes;
            CurrentValue = new byte[numBytes];
            lastValues = new byte[numBytes];
        }

        /// <summary>
        ///     Set up a ChainableSHiftRegisterOutput connected to another ChainableShiftRegisterOutput device
        /// </summary>
        /// <param name="upstreamDevice">The upstream device this device is attached to</param>
        /// <param name="numBytes">The number of bytes this device occupies in the chain</param>
        public ChainableShiftRegisterOutput(ChainableShiftRegisterOutput upstreamDevice, int numBytes = 1)
        {
            if (upstreamDevice.Parent == null)
                Parent = upstreamDevice;
            else
                Parent = upstreamDevice.Parent;

            ((ChainableShiftRegisterOutput)Parent).Children.Add(this);
            this.numBytes = numBytes;
            CurrentValue = new byte[numBytes];
            lastValues = new byte[numBytes];
        }

        /// <summary>
        ///     The current value of the port
        /// </summary>
        public byte[] CurrentValue { get; protected set; }

        /// <summary>
        ///     Whether or not written data should automatically be flushed to the controller
        /// </summary>
        public bool AutoFlush { get; set; } = true;

        /// <summary>
        ///     The parent shift register (if not null) this device is attached to
        /// </summary>
        public IFlushable Parent { get; protected set; }

        /// <summary>
        ///     Flush data to the port
        /// </summary>
        /// <param name="force">Whether to flush all data to the port, even if it doesn't appear to have changed.</param>
        /// <returns>An awaitable task that completes when finished</returns>
        public async Task FlushAsync(bool force = false)
        {
            if (!CurrentValue.SequenceEqual(lastValues) || force)
            {
                if(Parent != null)
                {
                    await Parent.FlushAsync();
                }
                else
                {
                    using (await lockObject.LockAsync())
                    {
                        // build the byte array to flush out of the port
                        var bytes = new List<byte>();

                        var reversed = Children.Reverse().ToList(); // bytes are shuffled out last-device-first
                        reversed.Add(this); // add ourselves

                        foreach (var shift in reversed)
                        {
                            var shiftBytes = shift.CurrentValue;
                            bytes.AddRange(shiftBytes.Reverse());
                        }

                        await spiDevice.SendReceiveAsync(bytes.ToArray(), SpiBurstMode.BurstTx);
                    }
                }
                CurrentValue.CopyTo(lastValues, 0);
            }
        }

        /// <summary>
        ///     Immediately update all the pins at once with the given value. Flush() will be implicity called.
        /// </summary>
        /// <param name="value">A value representing the data to write to the port</param>
        /// <returns>An awaitable task that completes upon successfully writing the value.</returns>
        public async Task Write(byte[] value)
        {
            var savedAutoFlushValue = AutoFlush;
            AutoFlush = false;
            CurrentValue = value;
            updateFromCurrentValue(); // tell our parent to update its pins, LEDs, or whatever.

            await FlushAsync(true);
            AutoFlush = savedAutoFlushValue;
        }

        /// <summary>
        ///     Classes extending this class should call this function after the internal pin data structure is updated.
        /// </summary>
        /// <returns></returns>
        protected async Task FlushIfAutoFlushEnabled()
        {
            if (AutoFlush)
                await FlushAsync();
        }

        /// <summary>
        ///     Update internal data structures from current value
        /// </summary>
        protected abstract void updateFromCurrentValue();
    }
}