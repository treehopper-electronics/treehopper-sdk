using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Treehopper.Utilities;

namespace Treehopper
{
/** Built-in SPI peripheral

SPI is a full-duplex synchronous serial interface useful for interfacing with both complex, high-speed peripherals, as well as simple LED drivers, output ports, and any other general-purpose input or output shift register.

Compared to I<sup>2</sup>, SPI is a simpler protocol, generally much faster, and less popular for modern peripheral ICs.

![Basic SPI interfacing](images/spi-overview.svg)

## Pins
%Treehopper supports SPI master mode with the following pins:
 - <b>MISO</b> <i>(Master In, Slave Out)</i>: This pin carries data from the slave to the master.
 - <b>MOSI</b> <i>(Master Out, Slave In)</i>: This pin carries data from the master to the peripheral
 - <b>SCK</b> <i>(Serial Clock)</i>: This pin clocks the data into and out of the master and slave device.

Not all devices use all pins, but the SPI peripheral will always reserve the SCK, MISO, and MOSI pin once the peripheral is enabled, so these pins cannot be used for other functions.

## Chip Select
Almost all SPI peripherals also use some sort of chip select (CS) pin, which indicates a valid transaction. Thus, the easiest way to place multiple peripherals on a bus is by using a separate chip select pin for each peripheral (since a peripheral will ignore SPI traffic without a valid chip select signal). %Treehopper supports two different chip-select styles:
 - SPI mode: chip-select is asserted at the beginning of a transaction, and de-asserted at the end; and
 - Shift output mode: chip-select is strobed at the end of a transaction
 - Shift input mode: chip-select is strobed at the beginning of a transaction
These styles support both active-low and active-high signal polarities.

## SPI Mode
SPI does not specify a transaction-level protocol for accessing peripheral functions (unlike, say, SMBus for I2c does); as a result, peripherals that use SPI have wildly different implementations. Even basic aspects -- when data is clocked, and the polarity of the clock signal -- vary by IC. This property is often called the "SPI mode" of the peripheral; %Treehopper supports all four modes:
 - <b>Mode 0 (00):</b> Clock is idle-low. Data is latched in on the clock's rising edge and data is output on the falling edge.
 - <b>Mode 1 (01):</b> Clock is idle-low. Data is latched in on the clock's falling edge and data is output on the rising edge.
 - <b>Mode 2 (10):</b> Clock is idle-high. Data is latched in on the clock's rising edge and data is output on the falling edge.
 - <b>Mode 3 (11):</b> Clock is idle-high. Data is latched in on the clock's falling edge and data is output on the rising edge.

## Clock Speed
%Treehopper supports SPI clock rates as low as 93.75 kHz and as high as 24 MHz, but we recommend a clock speed of 6 MHz for most cases. You will not notice performance gains above 6 MHz, since this is the fastest rate that %Treehopper's MCU can place bytes into the SPI buffer; any faster and the SPI peripheral will have to wait for the CPU before transmitting the next byte.

\note In the current firmware release, clock rates between 800 kHz and 6 MHz are disallowed. There appears to be a silicon bug in the SPI FIFO that can cause lock-ups with heavy USB traffic. We hope to create a workaround for this issue in future firmware updates.

## Chaining Devices & Shift Registers
%Treehopper's SPI module works well for interfacing with many types of shift registers, which typically have a single output state "register" that is updated whenever new SPI data comes in. Because of the nature of SPI, any existing data in this register is sent to the MISO pin (sometimes labeled "DO" --- digital output --- or, confusingly, "SO" --- serial output). Thus, many shift registers (even of different types) can be chained together by connecting the DO pin of each register to the DI pin of the next:

![Many shift registers can share the SPI bus and CS line](images/spi-shift-register.svg)
Please note that most shift registers refer to their "CS" pin as a "latch enable" (LE) signal.

In the example above, if both of these shift registers were 8-bit, sending the byte array {0xff, 0x03} would send "0xff" to the right register, and "0x03" to the left one. 

%Treehopper.Libraries has support for many different peripherals you can use with the %SPI peripheral, including shift registers. See the \ref libraries documentation for more details on all the library components. Examples of shift register library components include Treehopper.Libraries.Displays.LedShiftRegister, Treehopper.Libraries.Interface.Hc166, Treehopper.Libraries.Interface.Hc595.

 ## Further Reading
 Wikipedia has an excellent SPI article: [Serial Peripheral Interface Bus](https://en.wikipedia.org/wiki/Serial_Peripheral_Interface_Bus)
 */
    public class HardwareSpi : Spi
    {
        private readonly TreehopperUsb _device;
        private bool _enabled;

        internal HardwareSpi(TreehopperUsb device)
        {
            _device = device;
        }

        /** @name Main components
        @{
         */
        /// <summary>
        ///     Enable or disable the SPI module.
        /// </summary>
        /// <remarks>
        ///     When enabled, the MOSI, MISO, and SCK pins become reserved, and cannot be used for digital or analog operations.
        ///     When disabled, these pins return to an unassigned state.
        /// </remarks>
        public bool Enabled
        {
            get { return _enabled; }

            set
            {
                if (_enabled == value) return;
                _enabled = value;

                SendConfig();

                if (_enabled)
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
        ///     Send/receive data
        /// </summary>
        /// <param name="dataToWrite">
        ///     a byte array of the data to send. The length of the transaction is determined by the length
        ///     of this array.
        /// </param>
        /// <param name="chipSelect">The chip select pin, if any, to use during this transaction.</param>
        /// <param name="chipSelectMode">The chip select mode to use during this transaction (if a CS pin is selected)</param>
        /// <param name="speedMhz">The speed to perform this transaction at.</param>
        /// <param name="burstMode">Whether to use one of the burst modes</param>
        /// <param name="spiMode">The SPI mode to use during this transaction.</param>
        /// <returns>An awaitable byte array with the received data.</returns>
        public async Task<byte[]> SendReceiveAsync(byte[] dataToWrite, SpiChipSelectPin chipSelect = null,
            ChipSelectMode chipSelectMode = ChipSelectMode.SpiActiveLow, double speedMhz = 6,
            SpiBurstMode burstMode = SpiBurstMode.NoBurst, SpiMode spiMode = SpiMode.Mode00)
        {
            var transactionLength = dataToWrite.Length;
            var returnedData = new byte[transactionLength];

            if (Enabled != true)
                Utility.Error("SPI module must be enabled before starting transaction", true);

            if (chipSelect != null && chipSelect.SpiModule != this)
                Utility.Error("Chip select pin must belong to this SPI module", true);

            if (speedMhz > 0.8 && speedMhz < 6)
            {
                Debug.WriteLine(
                       "NOTICE: automatically rounding up SPI speed to 6 MHz, due to a possible silicon bug. This bug affects SPI speeds between 0.8 and 6 MHz, so if you need a speed lower than 6 MHz, please set to 0.8 MHz or lower.");

                speedMhz = 6;
            }

            using (await _device.ComsLock.LockAsync().ConfigureAwait(false))
            {
                var spi0Ckr = (int) Math.Round(24.0 / speedMhz - 1);
                if (spi0Ckr > 255.0)
                {
                    spi0Ckr = 255;
                    Debug.WriteLine(
                        "NOTICE: Requested SPI frequency of {0} MHz is below the minimum frequency, and will be clipped to 0.09375 MHz (93.75 kHz).",
                        speedMhz);
                }
                else if (spi0Ckr < 0)
                {
                    spi0Ckr = 0;
                    Debug.WriteLine(
                        "NOTICE: Requested SPI frequency of {0} MHz is above the maximum frequency, and will be clipped to 24 MHz.",
                        speedMhz);
                }

                var actualFrequency = 48.0 / (2.0 * (spi0Ckr + 1.0));

                if (Math.Abs(actualFrequency - speedMhz) > 1)
                    Debug.WriteLine(
                        "NOTICE: SPI module actual frequency of {0} MHz is more than 1 MHz away from the requested frequency of {1} MHz",
                        actualFrequency, speedMhz);

                if (dataToWrite.Length > 255)
                    throw new Exception("Maximum packet length is 255 bytes");

                var header = new byte[7];
                header[0] = (byte) DeviceCommands.SpiTransaction;
                header[1] = (byte) (chipSelect?.PinNumber ?? 255);
                header[2] = (byte) chipSelectMode;
                header[3] = (byte) spi0Ckr;
                header[4] = (byte) spiMode;
                header[5] = (byte) burstMode;
                header[6] = (byte) transactionLength;

                // just send the header
                if (burstMode == SpiBurstMode.BurstRx)
                {
                    await _device.SendPeripheralConfigPacketAsync(header).ConfigureAwait(false);
                }
                else
                {
                    var dataToSend = new byte[transactionLength + header.Length];
                    Array.Copy(header, dataToSend, header.Length);
                    Array.Copy(dataToWrite, 0, dataToSend, header.Length, transactionLength);

                    var bytesRemaining = dataToSend.Length;
                    var offset = 0;
                    while (bytesRemaining > 0)
                    {
                        var transferLength = bytesRemaining > 64 ? 64 : bytesRemaining;
                        var tmp = dataToSend.Skip(offset).Take(transferLength);
                        await _device.SendPeripheralConfigPacketAsync(tmp.ToArray()).ConfigureAwait(false);
                        offset += transferLength;
                        bytesRemaining -= transferLength;
                    }
                }

                // no need to wait if we're not reading anything
                if (burstMode != SpiBurstMode.BurstTx)
                {
                    var bytesRemaining = transactionLength;
                    var srcIndex = 0;
                    while (bytesRemaining > 0)
                    {
                        var numBytesToTransfer = bytesRemaining > 64 ? 64 : bytesRemaining;
                        var receivedData = await _device.ReceiveCommsResponsePacketAsync((uint) numBytesToTransfer)
                            .ConfigureAwait(false);
                        Array.Copy(receivedData, 0, returnedData, srcIndex,
                            receivedData.Length); // just in case we don't get what we're expecting
                        srcIndex += numBytesToTransfer;
                        bytesRemaining -= numBytesToTransfer;
                    }
                }
            }

            return returnedData;
        }

        /// @}


        /** @name Other components
            @{ */
        /// <summary>
        ///     Gets a string representing the SPI peripheral's state
        /// </summary>
        /// <returns>the state of the SPI peripheral</returns>
        public override string ToString()
        {
            if (Enabled)
                return "Enabled";
            return "Not enabled";
        }

        /// <summary>
        /// Gets the SCK pin of the board
        /// </summary>
        public Pin Sck
        {
            get
            {
                return _device.Pins[0];
            }
        }

        /// <summary>
        /// Gets the MISO pin of the board
        /// </summary>
        public Pin Miso
        {
            get
            {
                return _device.Pins[1];
            }
        }

        /// <summary>
        /// Gets the MOSI pin of the board
        /// </summary>
        public Pin Mosi
        {
            get
            {
                return _device.Pins[2];
            }
        }
        ///@}

        private void SendConfig()
        {
            var dataToSend = new byte[2];
            dataToSend[0] = (byte) DeviceCommands.SpiConfig;
            dataToSend[1] = (byte) (_enabled ? 0x01 : 0x00);
            _device.SendPeripheralConfigPacketAsync(dataToSend);
        }
    }
}