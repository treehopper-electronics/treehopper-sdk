using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.ThirdParty;

namespace Treehopper.Libraries.Interface.ShiftRegister
{
    /// <summary>
    /// Any shift-register-like device that can be daisy-chained onto other shift registers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note that this class doesn't expose a collection of <see cref="DigitalOutPin"/>s  or <see cref="Displays.Led"/>s, and instead, represents any writable shift register device. See <see cref="ShiftOut"/> for an implementation of a pin-based shift register. 
    /// </para>
    /// </remarks>
    public abstract class ChainableShiftRegisterOutput : IFlushable
    {
        /// <summary>
        /// Set up a ChainableShiftRegisterOutput connected to an SPI port.
        /// </summary>
        /// <param name="spiModule">the SPI module to use</param>
        /// <param name="latchPin">The latch pin to use</param>
        /// <param name="numBytes">The number of bytes to write to this device</param>
        /// <param name="mode">The SPI mode to use for all shift registers in this chain</param>
        /// <param name="csMode">The ChipSelectMode to use for all shift registers in this chain</param>
        /// <param name="speedMhz">The speed to use for all shift registers in this chain</param>
        public ChainableShiftRegisterOutput(Spi spiModule, SpiChipSelectPin latchPin, int numBytes = 1, double speedMhz = 5, SpiMode mode = SpiMode.Mode00, ChipSelectMode csMode = ChipSelectMode.PulseHighAtEnd)
        {
            setupSpi(spiModule, latchPin, speedMhz, mode, csMode);
            shiftRegisters.Add(this);
            this.numBytes = numBytes;
            CurrentValue = new byte[numBytes];
            lastValues = new byte[numBytes];
        }

        /// <summary>
        /// Set up a ChainableSHiftRegisterOutput connected to another ChainableShiftRegisterOutput device
        /// </summary>
        /// <param name="upstreamDevice">The upstream device this device is attached to</param>
        /// <param name="numBytes">The number of bytes this device occupies in the chain</param>
        public ChainableShiftRegisterOutput(ChainableShiftRegisterOutput upstreamDevice, int numBytes = 1)
        {
            if (upstreamDevice.Parent != null)
                this.Parent = upstreamDevice.Parent;
            else
                this.Parent = upstreamDevice;

            shiftRegisters.Add(this);
            this.numBytes = numBytes;
            CurrentValue = new byte[numBytes];
            lastValues = new byte[numBytes];
        }

        private void setupSpi(Spi spiModule, SpiChipSelectPin latchPin, double speedMhz, SpiMode mode, ChipSelectMode csMode)
        {
            spiDevice = new SpiDevice(spiModule, latchPin, csMode, speedMhz, mode);
        }

        /// <summary>
        /// Whether or not written data should automatically be flushed to the controller
        /// </summary>
        public bool AutoFlush { get; set; } = true;

        /// <summary>
        /// The current value of the port
        /// </summary>
        public byte[] CurrentValue { get; protected set; }

        public IFlushable Parent { get; protected set; }

        byte[] lastValues;


        /// <summary>
        /// Immediately update all the pins at once with the given value. Flush() will be implicity called.
        /// </summary>
        /// <param name="value">A value representing the data to write to the port</param>
        /// <returns>An awaitable task that completes upon successfully writing the value.</returns>
        public async Task Write(byte[] value)
        {
            bool savedAutoFlushValue = AutoFlush;
            AutoFlush = false;
            CurrentValue = value;
            updateFromCurrentValue(); // tell our parent to update its pins, LEDs, or whatever.

            await Flush(true);
            AutoFlush = savedAutoFlushValue;
        }

        /// <summary>
        /// Classes extending this class should call this function after the internal pin data structure is updated.
        /// </summary>
        /// <returns></returns>
        protected async Task FlushIfAutoFlushEnabled()
        {
            if (AutoFlush)
                await Flush();

        }

        /// <summary>
        /// Flush data to the port
        /// </summary>
        /// <param name="force">Whether to flush all data to the port, even if it doesn't appear to have changed.</param>
        /// <returns>An awaitable task that completes when finished</returns>
        public async Task Flush(bool force = false)
        {
            if (!CurrentValue.SequenceEqual(lastValues) || force)
            {
                await ShiftOut.requestWrite();
                CurrentValue.CopyTo(lastValues, 0);
            }
        }

        /// <summary>
        /// Update internal data structures from current value
        /// </summary>
        protected abstract void updateFromCurrentValue();

        private static SpiDevice spiDevice;

        private static Collection<ChainableShiftRegisterOutput> shiftRegisters = new Collection<ChainableShiftRegisterOutput>();
        private int numBytes;

        private static readonly AsyncLock lockObject = new AsyncLock();

        /// <summary>
        /// called to request a write to the device in chain (subsequently updating all devices in the chain)
        /// </summary>
        /// <returns></returns>
        protected static async Task requestWrite()
        {
            using(await lockObject.LockAsync())
            {
                // build the byte array to flush out of the port
                List<byte> bytes = new List<byte>();
                var reversedList = shiftRegisters.Reverse(); // we push out the last device's data first
                foreach (var shift in reversedList)
                {
                    var shiftBytes = shift.CurrentValue;
                    bytes.AddRange(shiftBytes.Reverse());
                }

                await spiDevice.SendReceive(bytes.ToArray(), BurstMode.BurstTx);
            }
            

        }
    }
}
