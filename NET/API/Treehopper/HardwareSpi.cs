namespace Treehopper
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides access to SPI communication.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Treehopper is capable of communicating with many devices equipped with serial peripheral interface (SPI) communication. 
    /// 
    /// SPI is a full-duplex synchronous serial communication standard that uses four pins:
    /// <list type="number">
    /// <item><term>MISO</term><description>Short for "Master In, Slave Out." This pin carries data from the device to the Treehopper. It is on pin 1</description></item>
    /// <item><term>MOSI</term><description>Short for "Master Out, Slave In." This pin carries data from the Treehopper to the device. It is on pin 2</description></item>
    /// <item><term>SCK</term><description>Short for "Serial Clock." This pin carries the clock signal from the Treehopper to the device. It is on pin 0</description></item>
    /// <item><term>CS</term><description>Short for "Chip Select." Treehopper asserts this pin when communication begins, and de-asserts when communication is done.</description></item>
    /// </list>
    /// The SPI specification allows for many different configuration options, so the datasheet for the device must be consulted to determine the communication rate, the
    /// clock phase and polarity, as well as the chip select polarity. Not all devices use all pins, but the SPI peripheral will always allocate the SCK, MISO, and MOSI pin once the peripheral is enabled.
    /// </para>
    /// <para>
    /// The clock rate to operate the SPI bus at is specified by the <see cref="SendReceive(byte[], SpiChipSelectPin, ChipSelectMode, double, BurstMode, SpiMode)"/> function. The minimum clock rate is 93.75 kHz (0.093.75 MHz), while the maximum clock rate is 24 MHz, but there are no performance gains above 6 MHz. Since Treehopper's MCU has no internal DMA, bytes are placed into the SPI buffer one by one by the processor; it takes 8 cycles to perform this operation, and since the processor runs at 48 MHz, the fastest effective data transfer rate is 6 MHz.
    /// </para>
    /// <para>
    /// Many simple devices that contain shift registers may also be interfaced with using the SPI module. The <see cref="ChipSelectMode"/> configuration contains pulse modes compatible with these devices.
    /// </para>
    /// <para>
    /// Before implementing SPI communication for a given device, check the Treehopper.Libraries assembly, which contains support for popular hobbyist devices, as well as generic
    /// shift registers and latches.
    /// </para>
    /// </remarks>
    public class HardwareSpi : Spi
    {
        private TreehopperUsb device;
        private bool enabled;

        internal HardwareSpi(TreehopperUsb device)
        {
            this.device = device;
        }

        /// <summary>
        /// Enable or disable the SPI module.
        /// </summary>
        /// <remarks>
        /// When enabled, the MOSI, MISO, and SCK pins become reserved, and cannot be used for digital or analog operations. When disabled, these pins return to an unassigned state.
        /// </remarks>
        public bool Enabled
        {
            get
            {
                return enabled;
            }

            set
            {
                if (enabled == value) return;
                enabled = value;

                SendConfig();

                if (enabled)
                {
                    Mosi.Mode = PinMode.Reserved;
                    Miso.Mode = PinMode.Reserved;
                    Sck.Mode = PinMode.Reserved;
                }
                else
                {
                    Mosi.Mode = PinMode.Unassigned;
                    Miso.Mode = PinMode.Unassigned;
                    Sck.Mode = PinMode.Unassigned;
                }
            }
        }

        private Pin Sck
        {
            get { return device.Pins[0]; }
        }

        private Pin Miso
        {
            get { return device.Pins[1]; }
        }

        private Pin Mosi
        {
            get { return device.Pins[2]; }
        }

        /// <summary>
        /// Send/receive data
        /// </summary>
        /// <param name="dataToWrite">a byte array of the data to send. The length of the transaction is determined by the length of this array.</param>
        /// <param name="chipSelect">The chip select pin, if any, to use during this transaction.</param>
        /// <param name="chipSelectMode">The chip select mode to use during this transaction (if a CS pin is selected)</param>
        /// <param name="speedMhz">The speed to perform this transaction at.</param>
        /// <param name="burstMode">Whether to use one of the burst modes</param>
        /// <param name="spiMode">The SPI mode to use during this transaction.</param>
        /// <returns>An awaitable byte array with the received data.</returns>
        public async Task<byte[]> SendReceive(byte[] dataToWrite, SpiChipSelectPin chipSelect = null, ChipSelectMode chipSelectMode = ChipSelectMode.SpiActiveLow, double speedMhz = 1, BurstMode burstMode = BurstMode.NoBurst, SpiMode spiMode = SpiMode.Mode00)
        {
            int transactionLength = dataToWrite.Length;
            byte[] returnedData = new byte[transactionLength];

            if (chipSelect.SpiModule != this)
                Utilities.Error("Chip select pin must belong to this SPI module");

            using (await device.ComsLock.LockAsync())
            {
                int spi0ckr = (int)Math.Round((24.0 / speedMhz) - 1);
                if (spi0ckr > 255.0)
                {
                    spi0ckr = 255;
                    Debug.WriteLine("NOTICE: Requested SPI frequency of {0} MHz is below the minimum frequency, and will be clipped to 0.09375 MHz (93.75 kHz).", speedMhz);
                }
                else if (spi0ckr < 0)
                {
                    spi0ckr = 0;
                    Debug.WriteLine("NOTICE: Requested SPI frequency of {0} MHz is above the maximum frequency, and will be clipped to 24 MHz.", speedMhz);
                }

                double actualFrequency = 48.0 / (2.0 * (spi0ckr + 1.0));

                if (Math.Abs(actualFrequency - speedMhz) > 1)
                    Debug.WriteLine("NOTICE: SPI module actual frequency of {0} MHz is more than 1 MHz away from the requested frequency of {1} MHz", actualFrequency, speedMhz);

                if (dataToWrite.Length > 255)
                    throw new Exception("Maximum packet length is 255 bytes");

                byte[] receivedData;
                int srcIndex = 0;
                byte[] header = new byte[7];
                header[0] = (byte)DeviceCommands.SpiTransaction;
                header[1] = (byte)(chipSelect?.PinNumber ?? 255);
                header[2] = (byte)chipSelectMode;
                header[3] = (byte)spi0ckr;
                header[4] = (byte)spiMode;
                header[5] = (byte)burstMode; // burstMode
                header[6] = (byte)transactionLength;

                // just send the header
                if (burstMode == BurstMode.BurstRx)
                {
                    device.SendPeripheralConfigPacket(header);
                }
                else
                {
                    byte[] dataToSend = new byte[transactionLength + header.Length];
                    Array.Copy(header, dataToSend, header.Length);
                    Array.Copy(dataToWrite, 0, dataToSend, header.Length, transactionLength);

                    int bytesRemaining = dataToSend.Length;
                    int offset = 0;
                    while (bytesRemaining > 0)
                    {
                        int transferLength = bytesRemaining > 64 ? 64 : bytesRemaining;
                        var tmp = dataToSend.Skip(offset).Take(transferLength);
                        device.SendPeripheralConfigPacket(tmp.ToArray());
                        offset += transferLength;
                        bytesRemaining -= transferLength;
                    }
                }

                // no need to wait if we're not reading anything
                if (burstMode != BurstMode.BurstTx)
                {
                    int bytesRemaining = transactionLength;
                    srcIndex = 0;
                    while (bytesRemaining > 0)
                    {
                        int numBytesToTransfer = bytesRemaining > 64 ? 64 : bytesRemaining;
                        receivedData = await device.ReceiveCommsResponsePacket((uint)numBytesToTransfer).ConfigureAwait(false);
                        Array.Copy(receivedData, 0, returnedData, srcIndex, receivedData.Length); // just in case we don't get what we're expecting
                        srcIndex += numBytesToTransfer;
                        bytesRemaining -= numBytesToTransfer;
                    }
                }
            }

            return returnedData;
        }

        public override string ToString()
        {
            if (Enabled)
                return "Enabled";
            else
                return "Not enabled";
        }

        private void SendConfig()
        {
            byte[] dataToSend = new byte[2];
            dataToSend[0] = (byte)DeviceCommands.SpiConfig;
            dataToSend[1] = (byte)(enabled ? 0x01 : 0x00);
            device.SendPeripheralConfigPacket(dataToSend);
        }
    }
}
