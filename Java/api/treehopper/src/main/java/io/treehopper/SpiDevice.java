package io.treehopper;

import io.treehopper.enums.BurstMode;
import io.treehopper.enums.ChipSelectMode;
import io.treehopper.enums.PinMode;
import io.treehopper.enums.SpiMode;

/**
 * An SPI peripheral device
 */
public class SpiDevice {
    private final Pin chipSelect;
    private final HardwareSpi spi;
    private final double frequency;
    private final SpiMode mode;
    private final ChipSelectMode chipSelectMode;

    /**
     * Create a new SPI device with the given settings
     * @param spiModule the Spi module to use
     * @param chipSelect the chip select pin to use
     * @param csMode The chip select mode to use
     */
    public SpiDevice(HardwareSpi spiModule, Pin chipSelect, ChipSelectMode csMode) {
        this(spiModule, chipSelect, csMode, 1, SpiMode.Mode00);
    }

    /**
     * Create a new SPI device with the given settings
     * @param spiModule the Spi module to use
     * @param chipSelect the chip select pin to use
     * @param csMode The chip select mode to use
     * @param speedMHz the speed, in MHz, to use
     */
    public SpiDevice(HardwareSpi spiModule, Pin chipSelect, ChipSelectMode csMode, double speedMHz) {
        this(spiModule, chipSelect, csMode, speedMHz, SpiMode.Mode00);
    }

    /**
     * Create a new SPI device with the given settings
     * @param spiModule the Spi module to use
     * @param chipSelect the chip select pin to use
     * @param csMode The chip select mode to use
     * @param speedMHz the speed, in MHz, to use
     * @param mode the SPI mode to use
     */
    public SpiDevice(HardwareSpi spiModule, Pin chipSelect, ChipSelectMode csMode, double speedMHz, SpiMode mode) {
        this.chipSelectMode = csMode;
        this.chipSelect = chipSelect;
        this.spi = spiModule;
        this.frequency = speedMHz;
        this.mode = mode;

        this.chipSelect.setMode(PinMode.PushPullOutput);

        spi.setEnabled(true);
    }

    /**
     * Exchange data with the SPI peripheral
     * @param dataToSend the data to send
     * @return the data read from the device
     */
    public byte[] SendReceive(byte[] dataToSend) {
        byte[] retVal = new byte[dataToSend.length];

        // We need to lock this in case another thread tries to step in and do a transaction with different settings
        return spi.SendReceive(dataToSend, chipSelect, chipSelectMode, frequency, BurstMode.NoBurst, mode);
    }

    /**
     * Exchange data with the SPI peripheral
     * @param dataToSend the data to send
     * @param burst the burst mode to use with the device
     * @return the data read from the device
     */
    public byte[] SendReceive(byte[] dataToSend, BurstMode burst) {
        byte[] retVal = new byte[dataToSend.length];

        // We need to lock this in case another thread tries to step in and do a transaction with different settings
        return spi.SendReceive(dataToSend, chipSelect, chipSelectMode, frequency, burst, mode);
    }
}
