using System;

namespace Treehopper
{
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

        PinPolarity ChipSelectPolarity;
       
        TreehopperUSB device;
        Pin ChipSelect;

        internal Spi(TreehopperUSB device)
        {
            this.device = device;
        }

        private void SendConfig()
        {
            if(this.ChipSelect != null)
            {
                this.ChipSelect.MakeDigitalOutput();
                if (ChipSelectPolarity == PinPolarity.ActiveLow)
                    this.ChipSelect.DigitalValue = true;
                else
                    this.ChipSelect.DigitalValue = false;
            }

            double SPI0CKR = (24.0 / _frequency) - 1;
            if (SPI0CKR > 255.0)
            {
                throw new Exception("SPI Rate out of limits. Valid rate is 93.75 kHz - 24 MHz");
            }

            actualFrequency = 47 / (2 * SPI0CKR + 1);

            byte[] dataToSend = new byte[4];
            dataToSend[0] = (byte)DeviceCommands.SPIConfig;
            dataToSend[1] = (byte)(isEnabled ? 1 : 0);
            dataToSend[1] = (byte)Mode;
            dataToSend[2] = (byte)SPI0CKR;
            device.sendCommsConfigPacket(dataToSend);
        }

        /// <summary>
        /// The <see cref="Frequency" /> property's name.
        /// </summary>
        public const string FrequencyPropertyName = "Frequency";

        private double _frequency;

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

        bool isEnabled;

        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                if (isEnabled == value) return;
                isEnabled = value;
                SendConfig();
            }
        }

        /// <summary>
        /// Sends and Receives data. 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="dataToWrite"></param>
        /// <param name="numBytesToRead"></param>
        /// <returns>Nothing. When data is received, an event will be generated</returns>
        public byte[] SendReceive(byte[] dataToWrite)
        {
            if (dataToWrite.Length > 255)
                throw new Exception("Maximum packet length is 255 bytes");
            byte[] returnedData = new byte[dataToWrite.Length];
            byte[] dataToSend = new byte[2 + dataToWrite.Length];
            dataToSend[0] = (byte)DeviceCommands.SPITransaction;
            dataToSend[1] = (byte)dataToWrite.Length;
            dataToWrite.CopyTo(dataToSend, 2);
            if (ChipSelect != null)
            {
                if (ChipSelectPolarity == PinPolarity.ActiveLow)
                    ChipSelect.DigitalValue = false;
                else
                    ChipSelect.DigitalValue = true;
            }
            device.sendCommsConfigPacket(dataToSend);
            //Thread.Sleep(1);
            byte[] receivedData = device.receiveCommsResponsePacket();
            if (ChipSelect != null)
                ChipSelect.ToggleOutput();
            return receivedData;
        }


    }
}
