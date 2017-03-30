package io.treehopper.interfaces;

import io.treehopper.enums.BurstMode;
import io.treehopper.enums.ChipSelectMode;
import io.treehopper.enums.SpiMode;

/**
 * SPI interface
 */
public interface Spi {
    /**
     * Gets whether the SPI interface is enabled
     * @return true if the interface is enabled; false otherwise
     */
    boolean isEnabled();

    /**
     * Sets whether the SPI interface is enabled
     * @param enabled true to enable the interface; false to disable it
     */
    void setEnabled(boolean enabled);

    /**
     * Exchange data with an SPI slave
     * @param dataToWrite the data to write
     * @return The data read from the peripheral
     */
    public byte[] SendReceive(byte[] dataToWrite);

    /**
     * Exchange data with an SPI slave
     * @param dataToWrite the data to write
     * @param chipSelect the chip select pin to use
     * @param chipSelectMode the chip select mode to use
     * @return The data read from the peripheral
     */
    public byte[] SendReceive(byte[] dataToWrite, SpiChipSelectPin chipSelect, ChipSelectMode chipSelectMode);

    /**
     * Exchange data with an SPI slave
     * @param dataToWrite the data to write
     * @param chipSelect the chip select pin to use
     * @param chipSelectMode the chip select mode to use
     * @param speedMhz the speed, in MHz, to use
     * @return The data read from the peripheral
     */
    public byte[] SendReceive(byte[] dataToWrite, SpiChipSelectPin chipSelect, ChipSelectMode chipSelectMode, double speedMhz);

    /**
     * Exchange data with an SPI slave
     * @param dataToWrite the data to write
     * @param chipSelect the chip select pin to use
     * @param chipSelectMode the chip select mode to use
     * @param speedMhz the speed, in MHz, to use
     * @param burstMode the burst mode (if any) to use
     * @return The data read from the peripheral
     */
    public byte[] SendReceive(byte[] dataToWrite, SpiChipSelectPin chipSelect, ChipSelectMode chipSelectMode, double speedMhz, BurstMode burstMode);

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
    public byte[] SendReceive(byte[] dataToWrite, SpiChipSelectPin chipSelect, ChipSelectMode chipSelectMode, double speedMhz, BurstMode burstMode, SpiMode spiMode);
}