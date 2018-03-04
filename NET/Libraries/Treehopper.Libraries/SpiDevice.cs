using System.Threading.Tasks;

namespace Treehopper.Libraries
{
    /// <summary>
    ///     Represents a peripheral attached to the SPI bus
    /// </summary>
    public class SpiDevice
    {
        private readonly Spi _spi;

        /// <summary>
        ///     Construct an SPI device attached to a particular module
        /// </summary>
        /// <param name="spiModule">The module this device is attached to</param>
        /// <param name="chipSelect">The chip select pin used by this device</param>
        /// <param name="chipSelectMode">The ChipSelectMode to use with this device</param>
        /// <param name="speedMhz">The speed to operate this device at</param>
        /// <param name="mode">The SpiMode of this device</param>
        public SpiDevice(Spi spiModule, SpiChipSelectPin chipSelect,
            ChipSelectMode chipSelectMode = ChipSelectMode.SpiActiveLow, double speedMhz = 6,
            SpiMode mode = SpiMode.Mode00)
        {
            ChipSelectMode = chipSelectMode;
            ChipSelect = chipSelect;
            _spi = spiModule;
            Frequency = speedMhz;
            Mode = mode;

            _spi.Enabled = true;

            ChipSelect.MakeDigitalPushPullOutAsync();

            // Set the initial chip select state
            if (chipSelectMode == ChipSelectMode.PulseLowAtBeginning ||
                chipSelectMode == ChipSelectMode.PulseLowAtEnd || chipSelectMode == ChipSelectMode.SpiActiveLow)
                ChipSelect.DigitalValue = true;
            else
                ChipSelect.DigitalValue = false;
        }

        /// <summary>
        ///     Get or set the pin to use for chip-select duties.
        /// </summary>
        /// <remarks>
        ///     Almost every SPI peripheral chip has some sort of chip select (which may be called load, strobe, or enable,
        ///     depending on the type of chip). You can use any <see cref="Pin" /> for chip-select duties as long as it belongs to
        ///     the same board as this SPI peripheral (i.e., you can't use a pin from one Treehopper as a chip-select for the SPI
        ///     port on another Treehopper).
        ///     Chip-selects are controlled at the firmware, not peripheral, level, which offers quite a bit of flexibility in
        ///     choosing the behavior. Make sure to set <see cref="ChipSelectMode" /> properly for your device.
        /// </remarks>
        public SpiChipSelectPin ChipSelect { get; protected set; }

        /// <summary>
        ///     Get or set the SPI module's mode
        /// </summary>
        /// <remarks>
        ///     The SPI module supports the four SPI modes: 00, 01, 10, 11. See <see cref="SpiMode" /> for more info.
        /// </remarks>
        public SpiMode Mode { get; protected set; }

        /// <summary>
        ///     Gets or sets the Frequency, in MHz, that this device will use for SPI communication.
        /// </summary>
        /// <remarks>
        ///     The SPI module can operate from 0.09375 MHz (93.75 kHz) to 24 MHz. Setting Frequency outside of those limits will
        ///     result in clipping, plus a debug notice.
        ///     Note that SPI transfers are managed by the CPU, as Treehopper's MCU has no DMA. As a result, there are diminishing
        ///     performance returns above 8 MHz, as dead space starts appearing between bytes as the SPI module waits for the CPU
        ///     to push the next byte into the register.
        /// </remarks>
        public double Frequency { get; set; }

        /// <summary>
        ///     The chip select mode to use with transactions
        /// </summary>
        public ChipSelectMode ChipSelectMode { get; protected set; }

        /// <summary>
        ///     Start an SPI transaction
        /// </summary>
        /// <param name="dataToSend">The data to send</param>
        /// <param name="burst">The burst mode, if any, to use</param>
        /// <returns>Data received by the peripheral</returns>
        public Task<byte[]> SendReceiveAsync(byte[] dataToSend, SpiBurstMode burst = SpiBurstMode.NoBurst)
        {
            return _spi.SendReceiveAsync(dataToSend, ChipSelect, ChipSelectMode, Frequency, burst, Mode);
        }
    }
}