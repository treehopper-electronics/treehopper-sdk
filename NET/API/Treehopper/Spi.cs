using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Treehopper
{
    /// <summary>
    /// The SPI burst mode to use
    /// </summary>
    public enum BurstMode
    {
        /// <summary>
        /// No burst -- always read the same number of bytes as transmitted
        /// </summary>
        NoBurst,

        /// <summary>
        /// Transmit burst -- don't return any data read from the bus
        /// </summary>
        BurstTx,

        /// <summary>
        /// Receive burst -- ignore transmitted data above 53 bytes long, but receive the full number of bytes specified
        /// </summary>
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
    /// Defines the clock phase and polarity used for SPI communication
    /// </summary>
    /// <remarks>
    /// <para>The left number indicates the clock polarity (CPOL), while the right number indicates the clock phase (CPHA). Consult https://en.wikipedia.org/wiki/Serial_Peripheral_Interface_Bus#Clock_polarity_and_phase for more information.</para>
    /// <para>Note that the numeric values of this enum do not match the standard nomenclature, but instead match the value needed by Treehopper's MCU. Do not attempt to cast integers from/to this enum.</para>
    /// </remarks>
    public enum SPIMode
    {
        /// <summary>
        /// Clock is initially low; data is valid on the rising edge of the clock
        /// </summary>
        Mode00 = 0x00,

        /// <summary>
        /// Clock is initially low; data is valid on the falling edge of the clock
        /// </summary>
        Mode01 = 0x20,

        /// <summary>
        /// Clock is initially high; data is valid on the rising edge of the clock
        /// </summary>
        Mode10 = 0x10,

        /// <summary>
        /// Clock is initially high; data is valid on the falling edge of the clock
        /// </summary>
        Mode11 = 0x30
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
        TreehopperUsb device;

        Pin Sck { get { return device.Pins[0]; } }
        Pin Miso { get { return device.Pins[1]; } }
        Pin Mosi { get { return device.Pins[2]; } }




        internal Spi(TreehopperUsb device)
        {
            this.device = device;
        }



        private void SendConfig()
        {
            byte[] dataToSend = new byte[2];
            dataToSend[0] = (byte)DeviceCommands.SpiConfig;
            dataToSend[1] = (enabled ? (byte)1 : (byte)0);
            device.sendPeripheralConfigPacket(dataToSend);
        }

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

                    //if(chipSelect != null)
                    //    chipSelect.Mode = PinMode.Unassigned;
                }

            }
        }

        /// <summary>
        /// Send/receive data
        /// </summary>
        /// <param name="dataToWrite">a byte array of the data to send. The length of the transaction is determined by the length of this array.</param>
        /// <param name="spiMode">The SPI mode to use during this transaction.</param>
        /// <param name="chipSelect">The chip select pin, if any, to use during this transaction.</param>
        /// <param name="chipSelectMode">The chip select mode to use during this transaction (if a CS pin is selected)</param>
        /// <param name="speedMhz">The speed to perform this transaction at.</param>
        /// <param name="burstMode">Whether to use one of the burst modes</param>
        /// <returns>An awaitable byte array with the received data.</returns>
        public async Task<byte[]> SendReceive(byte[] dataToWrite, SPIMode spiMode, Pin chipSelect = null, ChipSelectMode chipSelectMode = ChipSelectMode.SpiActiveLow, double speedMhz = 1, BurstMode burstMode = BurstMode.NoBurst)
        {
            int transactionLength = dataToWrite.Length;
            byte[] returnedData = new byte[transactionLength];

            //lock (device.ComsLock)
            using(await device.ComsLock.LockAsync())
            {
                int SPI0CKR = (int)Math.Round((24.0 / speedMhz) - 1);
                if (SPI0CKR > 255.0)
                {
                    SPI0CKR = 255;
                    Debug.WriteLine("NOTICE: Requested SPI frequency of {0} MHz is below the minimum frequency, and will be clipped to 0.09375 MHz (93.75 kHz).", speedMhz);
                }
                else if (SPI0CKR < 0)
                {
                    SPI0CKR = 0;
                    Debug.WriteLine("NOTICE: Requested SPI frequency of {0} MHz is above the maximum frequency, and will be clipped to 24 MHz.", speedMhz);
                }

                double actualFrequency = 48.0 / (2.0 * (SPI0CKR + 1.0));

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
                header[3] = (byte)SPI0CKR;
                header[4] = (byte)spiMode;
                header[5] = (byte)burstMode; // burstMode
                header[6] = (byte)transactionLength;

                // just send the header
                if (burstMode == BurstMode.BurstRx)
                {
                    device.sendPeripheralConfigPacket(header);
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
                        device.sendPeripheralConfigPacket(tmp.ToArray());
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
                        //receivedData = device.receiveCommsResponsePacket((uint)numBytesToTransfer).Result;
                        receivedData = await device.receiveCommsResponsePacket((uint)numBytesToTransfer).ConfigureAwait(false);
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
