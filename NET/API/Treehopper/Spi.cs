using Nito.AsyncEx;
using System;
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
    /// <item><term>MISO</term><description>Short for "Master In, Slave Out." This pin carries data from the device to the Treehopper. It is on <see cref="Pin10"/></description></item>
    /// <item><term>MOSI</term><description>Short for "Master Out, Slave In." This pin carries data from the Treehopper to the device. It is on <see cref="Pin12"/></description></item>
    /// <item><term>SCK</term><description>Short for "Serial Clock." This pin carries the clock signal from the Treehopper to the device. It is on <see cref="Pin11"/></description></item>
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
        private bool noEvents;
        private bool dataAvailable;
        private byte[] receivedData;

        private readonly AsyncLock mutex = new AsyncLock();

        PinPolarity chipSelectPolarity = PinPolarity.ActiveLow;
        PinPolarity ChipSelectPolarity
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
            }
        }
       
        TreehopperUsb device;

        Pin Mosi { get { return device.Pin3; } }
        Pin Miso { get { return device.Pin2; } }
        Pin Sck  { get { return device.Pin1; } }

        Pin chipSelect;
        public Pin ChipSelect {
            get
            {
                return chipSelect;
            } set
            {
                if (value == chipSelect)
                    return;

                if(chipSelect != null)
                    chipSelect.Mode = PinMode.DigitalInput; // make the old chip select pin a digital input

                chipSelect = value;

                if (chipSelect.Mode == PinMode.Reserved)
                {
                    throw new Exception("The specified ChipSelect pin is already in use by another peripheral");
                }
                else if (chipSelect.Board != device)
                {
                    throw new Exception("The specified ChipSelect pin does not belong to this TreehopperUsb device");
                }
                else if (chipSelect != null)
                {
                    //chipSelect.Mode = PinMode.Reserved; 
                }
            }
        }

        internal Spi(TreehopperUsb device)
        {
            this.device = device;
        }

        private void SendConfig()
        {


            double SPI0CKR = (24.0 / _frequency) - 1;
            if (SPI0CKR > 255.0)
            {
                throw new Exception("SPI Rate out of limits. Valid rate is 93.75 kHz - 24 MHz");
            }

            actualFrequency = 47 / (2 * SPI0CKR + 1);

            byte[] dataToSend = new byte[6];
            dataToSend[0] = (byte)DeviceCommands.SpiConfig;
            dataToSend[1] = (enabled ? (byte)1 : (byte)0);
            dataToSend[2] = (byte)Mode;
            dataToSend[3] = (byte)SPI0CKR;
            dataToSend[4] = (byte)(chipSelect == null ? 0 : chipSelect.PinNumber);
            dataToSend[5] = (byte)(chipSelectPolarity == PinPolarity.ActiveHigh ? 1 : 0);
            device.sendPeripheralConfigPacket(dataToSend);
        }

        /// <summary>
        /// The <see cref="Frequency" /> property's name.
        /// </summary>
        public const string FrequencyPropertyName = "Frequency";

        private double _frequency = 6;

        /// <summary>
        /// Sets and gets the Frequency property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double Frequency
        {
            get
            {
                return _frequency;
            }

            set
            {
                //if (_frequency == value) return; // Don't try comparing floats
                _frequency = value;
                SendConfig();
            }
        }

        private SPIMode mode;
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

        double actualFrequency = 0;
        public double ActualFrequency
        {
            get {
                return actualFrequency;
            }
            
        }

        bool enabled;

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
            using (await mutex.LockAsync())
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
                if (mode == BurstMode.BurstTx)
                    return returnedData;

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
                
            return returnedData;
        }


    }
}
