package io.treehopper.interfaces;

/**
 * A Digital Out pin that can be used as an SPI chip select pin
 */
public interface SpiChipSelectPin {
    int getPinNumber();

    Spi getSpiModule();
}
