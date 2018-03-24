package io.treehopper;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import io.treehopper.enums.BurstMode;
import io.treehopper.enums.ChipSelectMode;
import io.treehopper.enums.PinMode;
import io.treehopper.enums.SpiMode;
import io.treehopper.interfaces.Spi;
import io.treehopper.interfaces.SpiChipSelectPin;

/**
 * TreehopperUsb SPI interface
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
