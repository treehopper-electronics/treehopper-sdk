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
        public ChainableShiftRegisterOutput(Spi spiModule, Pin latchPin, int numBytes = 1, SPIMode mode = SPIMode.Mode00, ChipSelectMode csMode = ChipSelectMode.PulseHighAtEnd, double speedMhz = 1)
        {
            setupSpi(spiModule, latchPin, speedMhz, mode, csMode);
            shiftRegisters.Add(this);
            this.numBytes = numBytes;
        }

        public ChainableShiftRegisterOutput(ChainableShiftRegisterOutput upstreamDevice, int numBytes = 1)
        {
            shiftRegisters.Add(this);
            this.numBytes = numBytes;
        }

        private void setupSpi(Spi spiModule, Pin latchPin, double speedMhz, SPIMode mode, ChipSelectMode csMode)
        {
            spiDevice = new SpiDevice(spiModule, latchPin, speedMhz, mode, csMode);
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
            CurrentValue = value;
            updateFromCurrentValue(); // tell our parent to update its pins, LEDs, or whatever.

            await Flush();
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

        public async Task Flush(bool force = false)
        {
            if (CurrentValue != lastValues || force)
            {
                await ShiftOut.requestWrite();
                lastValues = CurrentValue;
            }
        }

        protected abstract void updateFromCurrentValue();

        private static SpiDevice spiDevice;

        private static Collection<ChainableShiftRegisterOutput> shiftRegisters = new Collection<ChainableShiftRegisterOutput>();
        private int numBytes;

        private static readonly AsyncLock lockObject = new AsyncLock();
        protected static async Task requestWrite()
        {
            using(await lockObject.LockAsync())
            {
                // build the byte array to flush out of the port
                List<byte> bytes = new List<byte>();
                var reversedList = shiftRegisters.Reverse(); // we push out the last device's data first
                foreach (var shift in reversedList)
                {
                    var shiftBytes = BitConverter.GetBytes(shift.CurrentValue);

                    // who knows, maybe we're not on x86? Check just to be sure!
                    if (BitConverter.IsLittleEndian)
                    {
                        shiftBytes = shiftBytes.Reverse().ToArray();
                    }
                    else
                    {

                    }


                    // We'll always end up with 4 bytes, but we can't send more bytes than our shift register is expecting (8, 16, etc)
                    // note that the 
                    for (int i = 4 - shift.numBytes; i < 4; i++)
                    {
                        bytes.Add(shiftBytes[i]);
                    }
                }

                await spiDevice.SendReceive(bytes.ToArray(), BurstMode.BurstTx);
            }
            

        }
    }
}
