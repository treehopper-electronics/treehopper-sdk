using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Treehopper
{
    public enum BurstMode
    {
        NoBurst,
        BurstTx,
        BurstRx
    }

    /// <summary>
    /// Defines whether a signal is active high (rising-edge) or active low (falling-edge)
    /// </summary>
    public enum ChipSelectMode
    {

        /// <summary>
        /// CS is asserted low, the SPI transaction takes place, and then the signal is returned high.
        /// </summary>
        SpiActiveLow,
        /// <summary>
        /// CS is asserted high, the SPI transaction takes place, and then the signal is returned low.
        /// </summary>
        SpiActiveHigh,

        /// <summary>
        /// CS is pulsed high, and then the SPI transaction takes place.
        /// </summary>
        PulseHighAtBeginning,

        /// <summary>
        /// The SPI transaction takes place, and once finished, CS is pulsed high
        /// </summary>
        PulseHighAtEnd,

        /// <summary>
        /// CS is pulsed low, and then the SPI transaction takes place.
        /// </summary>
        PulseLowAtBeginning,

        /// <summary>
        /// The SPI transaction takes place, and once finished, CS is pulsed low
        /// </summary>
        PulseLowAtEnd



    };

    /// <summary>
    /// Defines when incoming data is sampled
    /// </summary>
    public enum SPISampleMode
    {
        /// <summary>
        /// Incoming data is sampled in the middle of the period where outgoing data is valid (default)
        /// </summary>
        Middle,

        /// <summary>
        /// Incoming data is sampled at the end of the period where outgoing data is valid.
        /// </summary>
        End
    };

    /// <summary>
    /// Defines the clock phase and polarity used for SPI communication
    /// </summary>
    /// <remarks>
    /// The left number indicates the clock polarity (CPOL), while the right number indicates the clock phase (CPHA). Consult https://en.wikipedia.org/wiki/Serial_Peripheral_Interface_Bus#Clock_polarity_and_phase for more information.
    /// </remarks>
    public enum SPIMode
    {
        /// <summary>
        /// Clock is initially low; data is valid on the rising edge of the clock
        /// </summary>
        Mode00,

        /// <summary>
        /// Clock is initially low; data is valid on the falling edge of the clock
        /// </summary>
        Mode01,

        /// <summary>
        /// Clock is initially high; data is valid on the rising edge of the clock
        /// </summary>
        Mode10,

        /// <summary>
        /// Clock is initially high; data is valid on the falling edge of the clock
        /// </summary>
        Mode11
    };

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
    /// clock phase and polarity, as well as the chip select polarity.
    /// </para>
    /// <para>
    /// Many simple devices that contain latches or shift registers may also be interfaced with using the SPI module.
    /// </para>
    /// <para>
    /// Before implementing SPI communication for a given device, check the Treehopper.Libraries assembly, which contains support for popular hobbyist devices, as well as generic
    /// shift registers and latches.
    /// </para>
    /// </remarks>
    public class Spi
    {
        ChipSelectMode chipSelectPolarity = ChipSelectMode.SpiActiveLow;

        /// <summary>
        /// Gets or sets the chip select polarity of the SPI module.
        /// </summary>
        public ChipSelectMode ChipSelectMode
        {
            get
            {
                return chipSelectPolarity;
            }
            set
            {
                if (chipSelectPolarity == value)
                    return;
                chipSelectPolarity = value;

                if(Enabled)
                    SendConfig(); // only send the config when we're enabled.
            }
        }
       
        TreehopperUsb device;

        Pin Sck { get { return device.Pins[0]; } }
        Pin Miso { get { return device.Pins[1]; } }
        Pin Mosi { get { return device.Pins[2]; } }



        Pin chipSelect;

        /// <summary>
        /// Get or set the pin to use for chip-select duties.
        /// </summary>
        /// <remarks>
        /// Almost every SPI peripheral chip has some sort of chip select (which may be called load, strobe, or enable, depending on the type of chip). You can use any <see cref="Pin"/> for chip-select duties as long as it belongs to the same board as this SPI peripheral (i.e., you can't use a pin from one Treehopper as a chip-select for the SPI port on another Treehopper).
        /// Chip-selects are controlled at the firmware, not peripheral, level, which offers quite a bit of flexibility in choosing the behavior. Make sure to set <see cref="ChipSelectMode"/> properly for your device.
        /// </remarks>
        public Pin ChipSelect {
            get
            {
                return chipSelect;
            } set
            {
                if (value == chipSelect)
                    return;

                if(chipSelect != null)
                    chipSelect.Mode = PinMode.Unassigned; // unassign old chip select pin

                if (value.Board != device)
                {
                    throw new Exception("The specified ChipSelect pin does not belong to this TreehopperUsb device");
                }
                else if (chipSelect != null)
                {
                    chipSelect.Mode = PinMode.Reserved;
                }

                // finally, assign the new CS pin
                chipSelect = value;

                if (Enabled)
                    SendConfig(); // only send the config when we're enabled.
            }
        }

        internal Spi(TreehopperUsb device)
        {
            this.device = device;
        }

        private void SendConfig()
        {


            int SPI0CKR = (int)Math.Round((24.0 / _frequency) - 1);
            if (SPI0CKR > 255.0)
            {
                SPI0CKR = 255;
                Debug.WriteLine("NOTICE: Requested SPI frequency of {0} MHz is below the minimum frequency, and will be clipped to 0.09375 MHz (93.75 kHz).", Frequency);
            }
            else if(SPI0CKR < 0)
            {
                SPI0CKR = 0;
                Debug.WriteLine("NOTICE: Requested SPI frequency of {0} MHz is above the maximum frequency, and will be clipped to 24 MHz.", Frequency);
            }

            ActualFrequency = 48.0 / (2.0 * (SPI0CKR + 1.0));

            if (Math.Abs(ActualFrequency - Frequency) > 1)
                Debug.WriteLine("NOTICE: SPI module actual frequency of {0} MHz is more than 1 MHz away from the requested frequency of {1} MHz", ActualFrequency, Frequency);

            byte[] dataToSend = new byte[6];
            dataToSend[0] = (byte)DeviceCommands.SpiConfig;
            dataToSend[1] = (enabled ? (byte)1 : (byte)0);
            dataToSend[2] = (byte)Mode;
            dataToSend[3] = (byte)SPI0CKR;
            dataToSend[4] = (byte)(chipSelect == null ? 255 : chipSelect.PinNumber); // send an invalid pin number (255) if we're not using chipSelect
            dataToSend[5] = (byte)(chipSelectPolarity);
            device.sendPeripheralConfigPacket(dataToSend);
        }

        /// <summary>
        /// The <see cref="Frequency" /> property's name.
        /// </summary>
        public const string FrequencyPropertyName = "Frequency";

        private double _frequency = 6;

        /// <summary>
        /// Sets and gets the Frequency, in MHz, of the SPI module.
        /// </summary>
        /// <remarks>
        /// The SPI module can operate from 0.09375 MHz (93.75 kHz) to 24 MHz. Setting Frequency outside of those limits will result in clipping, plus a debug notice.
        /// Note that SPI transfers are managed by the CPU, as Treehopper's MCU has no DMA. As a result, there are diminishing performance returns above 8 MHz, as dead space starts appearing between bytes as the SPI module waits for the CPU to push the next byte into the register.
        /// </remarks>
        public double Frequency
        {
            get
            {
                return _frequency;
            }

            set
            {
                // if we're basically setting the same frequency, no need to update it.
                if (Math.Abs(_frequency-value) < 0.01) return;
                _frequency = value;
                SendConfig();
            }
        }

        private SPIMode mode;

        /// <summary>
        /// Get or set the SPI module's mode
        /// </summary>
        /// <remarks>
        /// The SPI module supports the four SPI modes: 00, 01, 10, 11. See <see cref="SPIMode"/> for more info.
        /// </remarks>
        public SPIMode Mode { 
            get
            {
                return mode;
            }
            set
            {
                if (mode == value) return;
                mode = value;
                SendConfig();
            }
        }

        /// <summary>
        /// Get the actual frequency the SPI module is operating at (after clipping and rounding is applied).
        /// </summary>
        /// <remarks>
        /// To learn more about the SPI module's frequency capability, <see cref="Frequency"/> .
        /// </remarks>
        public double ActualFrequency { get; private set; } = 0;


        bool enabled;

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

                    if(chipSelect != null)
                        chipSelect.Mode = PinMode.Unassigned;
                }

            }
        }

        /// <summary>
        /// Sends and Receives data. 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="dataToWrite"></param>
        /// <param name="numBytesToRead"></param>
        /// <returns>Nothing. When data is received, an event will be generated</returns>
        public async Task<byte[]> SendReceive(byte[] dataToWrite, BurstMode mode = BurstMode.NoBurst)
        {
            int transactionLength = dataToWrite.Length;
            if (dataToWrite.Length > 255)
                throw new Exception("Maximum packet length is 255 bytes");
            byte[] returnedData = new byte[dataToWrite.Length];
            using (await device.ComsMutex.LockAsync())
            {
                byte[] receivedData;
                int srcIndex = 0;
                int bytesRemaining = transactionLength;
                while (bytesRemaining > 0)
                {
                    int numBytesToTransfer = bytesRemaining > 59 ? 59 : bytesRemaining;
                    byte[] dataToSend = new byte[5 + numBytesToTransfer]; // 2 bytes for the header
                    dataToSend[0] = (byte)DeviceCommands.SpiTransaction;
                    dataToSend[1] = (byte)dataToWrite.Length;
                    dataToSend[2] = (byte)srcIndex; // offset
                    dataToSend[3] = (byte)numBytesToTransfer; // count
                    dataToSend[4] = (byte)mode; // burstMode
                    Array.Copy(dataToWrite, srcIndex, dataToSend, 5, numBytesToTransfer);
                    device.sendPeripheralConfigPacket(dataToSend);
                    srcIndex += numBytesToTransfer;
                    bytesRemaining -= numBytesToTransfer;
                    if (mode == BurstMode.BurstRx) // don't send additional data, just wait for read
                        break;
                }

                // no need to wait if we're not reading anything
                if (mode != BurstMode.BurstTx)
                {
                    bytesRemaining = transactionLength;
                    srcIndex = 0;
                    while (bytesRemaining > 0)
                    {
                        int numBytesToTransfer = bytesRemaining > 64 ? 64 : bytesRemaining;
                        receivedData = await device.receiveCommsResponsePacket((uint)numBytesToTransfer);
                        Array.Copy(receivedData, 0, returnedData, srcIndex, receivedData.Length); // just in case we don't get what we're expecting
                        srcIndex += numBytesToTransfer;
                        bytesRemaining -= numBytesToTransfer;
                    }
                }
            }
                
            return returnedData;
        }


    }
}
