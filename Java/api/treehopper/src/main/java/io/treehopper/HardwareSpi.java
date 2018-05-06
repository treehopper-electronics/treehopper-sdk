package io.treehopper;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import io.treehopper.enums.BurstMode;
import io.treehopper.enums.ChipSelectMode;
import io.treehopper.enums.PinMode;
import io.treehopper.enums.SpiMode;
import io.treehopper.interfaces.Spi;
import io.treehopper.interfaces.SpiChipSelectPin;

/** Built-in SPI peripheral

# Basic Usage
Once enabled(), you can use the hardware SPI module on %Treehopper through the sendReceive() method, which is used to simultaneously transmit and/or receive data.

# Background
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

## Burst mode
If you only need to transmit or receive data from the device, %Treehopper supports an \link io.treehopper.enums.BurstMode BurstMode\endlink flag, which can improve performance substantially (especially in the case of BurstTx, which eliminates the back-and-forth needed, reducing transaction times down to a few hundred microseconds).

## Chaining Devices & Shift Registers
%Treehopper's SPI module works well for interfacing with many types of shift registers, which typically have a single output state "register" that is updated whenever new SPI data comes in. Because of the nature of SPI, any existing data in this register is sent to the MISO pin (sometimes labeled "DO" --- digital output --- or, confusingly, "SO" --- serial output). Thus, many shift registers (even of different types) can be chained together by connecting the DO pin of each register to the DI pin of the next:

![Many shift registers can share the SPI bus and CS line](images/spi-shift-register.svg)
Please note that most shift registers refer to their "CS" pin as a "latch enable" (LE) signal.

In the example above, if both of these shift registers were 8-bit, sending the byte array {0xff, 0x03} would send "0xff" to the right register, and "0x03" to the left one.

%Treehopper.Libraries has support for many different peripherals you can use with the %SPI peripheral, including shift registers. See the \ref libraries documentation for more details on all the library components. Examples of shift register library components include Treehopper.Libraries.Displays.LedShiftRegister, Treehopper.Libraries.IO.PortExpander.Hc166, Treehopper.Libraries.IO.PortExpander.Hc595.

 ## Further Reading
 Wikipedia has an excellent SPI article: [Serial Peripheral Interface Bus](https://en.wikipedia.org/wiki/Serial_Peripheral_Interface_Bus)
 */
public class HardwareSpi implements Spi {

    static final Logger logger = LogManager.getLogger("SPI");

    boolean enabled;
    private TreehopperUsb board;

    HardwareSpi(TreehopperUsb board) {
        this.board = board;
    }

    /**
     * Gets whether the SPI interface is enabled
     * @return true if the interface is enabled; false otherwise
     */
    public boolean isEnabled() {
        return enabled;
    }

    /**
     * Sets whether the SPI interface is enabled
     * @param enabled true to enable the interface; false to disable it
     */
    public void setEnabled(boolean enabled) {
        if (this.enabled == enabled) return;

        this.enabled = enabled;

        // Update config
        byte[] dataToSend = new byte[2];
        dataToSend[0] = (byte) DeviceCommands.SpiConfig.ordinal();
        dataToSend[1] = (enabled ? (byte) 1 : (byte) 0);
        board.sendPeripheralConfigPacket(dataToSend);

        // mark pins correctly
        if (enabled) {
            board.pins[0].setMode(PinMode.Reserved);
            board.pins[1].setMode(PinMode.Reserved);
            board.pins[2].setMode(PinMode.Reserved);
        } else {
            board.pins[0].setMode(PinMode.Unassigned);
            board.pins[1].setMode(PinMode.Unassigned);
            board.pins[2].setMode(PinMode.Unassigned);
        }
    }

    /**
     * Exchange data with an SPI slave
     * @param dataToWrite the data to write
     * @return The data read from the peripheral
     */
    public byte[] SendReceive(byte[] dataToWrite) {
        return SendReceive(dataToWrite, null, ChipSelectMode.SpiActiveLow, 6, BurstMode.NoBurst, SpiMode.Mode00);
    }

    /**
     * Exchange data with an SPI slave
     * @param dataToWrite the data to write
     * @param chipSelect the chip select pin to use
     * @param chipSelectMode the chip select mode to use
     * @return The data read from the peripheral
     */
    public byte[] SendReceive(byte[] dataToWrite, SpiChipSelectPin chipSelect, ChipSelectMode chipSelectMode) {
        return SendReceive(dataToWrite, chipSelect, chipSelectMode, 1, BurstMode.NoBurst, SpiMode.Mode00);
    }

    /**
     * Exchange data with an SPI slave
     * @param dataToWrite the data to write
     * @param chipSelect the chip select pin to use
     * @param chipSelectMode the chip select mode to use
     * @param speedMhz the speed, in MHz, to use
     * @return The data read from the peripheral
     */
    public byte[] SendReceive(byte[] dataToWrite, SpiChipSelectPin chipSelect, ChipSelectMode chipSelectMode, double speedMhz) {
        return SendReceive(dataToWrite, chipSelect, chipSelectMode, speedMhz, BurstMode.NoBurst, SpiMode.Mode00);
    }

    /**
     * Exchange data with an SPI slave
     * @param dataToWrite the data to write
     * @param chipSelect the chip select pin to use
     * @param chipSelectMode the chip select mode to use
     * @param speedMhz the speed, in MHz, to use
     * @param burstMode the burst mode (if any) to use
     * @return The data read from the peripheral
     */
    public byte[] SendReceive(byte[] dataToWrite, SpiChipSelectPin chipSelect, ChipSelectMode chipSelectMode, double speedMhz, BurstMode burstMode) {
        return SendReceive(dataToWrite, chipSelect, chipSelectMode, speedMhz, burstMode, SpiMode.Mode00);
    }

    /**
     * Exchange data with an SPI slave
     * @param dataToWrite the data to write
     * @param chipSelect the chip select pin to use
     * @param chipSelectMode the chip select mode to use
     * @param speedMhz the speed, in MHz, to use
     * @param burstMode the burst mode (if any) to use
     * @param spiMode the SPI mode to use
     * @return The data read from the peripheral
     */
    public byte[] SendReceive(byte[] dataToWrite, SpiChipSelectPin chipSelect, ChipSelectMode chipSelectMode, double speedMhz, BurstMode burstMode, SpiMode spiMode) {
        if (!enabled) {
            String message = "I2c.SendReceive() called before enabling the peripheral. This call will be ignored.";
            logger.error(message);
            if (TreehopperUsb.Settings.shouldThrowExceptions()) {
                throw new RuntimeException(message);
            }
        }

        if(speedMhz > 0.8 && speedMhz < 6)
        {
            logger.info("NOTICE: automatically rounding up SPI speed to 6 MHz, due to a possible silicon bug. " +
                    "This bug affects SPI speeds between 0.8 and 6 MHz, so if you need a speed lower than 6 MHz, " +
                    "please set to 0.8 MHz or lower.");
            speedMhz = 6;
        }

        if (dataToWrite.length > 255) {
            logger.error("You may only receive up to 255 bytes per transaction.");
            if (TreehopperUsb.Settings.shouldThrowExceptions()) {
                throw new IllegalArgumentException("You may only receive up to 255 bytes per transaction.");
            }
        }

        int transactionLength = dataToWrite.length;
        byte[] returnedData = new byte[transactionLength];

        synchronized (board.comsLock) {
            int SPI0CKR = (int) Math.round((24.0 / speedMhz) - 1);
            if (SPI0CKR > 255.0) {
                SPI0CKR = 255;
                logger.error("Requested SPI frequency of {0} MHz is below the minimum frequency, and will be clipped to 0.09375 MHz (93.75 kHz).", speedMhz);
            } else if (SPI0CKR < 0) {
                SPI0CKR = 0;
                logger.error("Requested SPI frequency of {0} MHz is above the maximum frequency, and will be clipped to 24 MHz.", speedMhz);
            }

            double actualFrequency = 48.0 / (2.0 * (SPI0CKR + 1.0));

            if (Math.abs(actualFrequency - speedMhz) > 1)
                logger.error("SPI module actual frequency of {0} MHz is more than 1 MHz away from the requested frequency of {1} MHz", actualFrequency, speedMhz);


            byte[] receivedData;
            int srcIndex = 0;


            byte[] header = new byte[7];
            header[0] = (byte) DeviceCommands.SpiTransaction.ordinal();
            header[1] = (byte) (chipSelect != null ? chipSelect.getPinNumber() : 255);
            header[2] = (byte) chipSelectMode.ordinal();
            header[3] = (byte) SPI0CKR;
            header[4] = spiMode.getModeRegisterValue();
            header[5] = (byte) burstMode.ordinal(); // burstMode
            header[6] = (byte) transactionLength;

            // just send the header
            if (burstMode == BurstMode.BurstRx) {
                board.sendPeripheralConfigPacket(header);
            } else {
                byte[] dataToSend = new byte[transactionLength + header.length];
                System.arraycopy(header, 0, dataToSend, 0, header.length);
                System.arraycopy(dataToWrite, 0, dataToSend, header.length, transactionLength);

                int bytesRemaining = dataToSend.length;
                int offset = 0;

                // for long transactions (> 64 bytes - 4 byte header), we send <=64 byte chunks, one by one.
                while (bytesRemaining > 0) {
                    int transferLength = bytesRemaining > 64 ? 64 : bytesRemaining;
                    byte[] tmp = new byte[transferLength];
                    System.arraycopy(dataToSend, offset, tmp, 0, transferLength);
                    board.sendPeripheralConfigPacket(tmp);

                    offset += transferLength;
                    bytesRemaining -= transferLength;
                }
            }

            // no need to wait if we're not reading anything
            if (burstMode != BurstMode.BurstTx) {
                int bytesRemaining = transactionLength;
                srcIndex = 0;
                while (bytesRemaining > 0) {
                    int numBytesToTransfer = bytesRemaining > 64 ? 64 : bytesRemaining;
                    receivedData = board.receiveCommsResponsePacket(numBytesToTransfer);
                    System.arraycopy(receivedData, 0, returnedData, srcIndex, receivedData.length); // just in case we don't get what we're expecting
                    srcIndex += numBytesToTransfer;
                    bytesRemaining -= numBytesToTransfer;
                }
            }
        }

        return returnedData;
    }
}
