package io.treehopper.enums;

/**
 * Defines whether a signal is active high (rising-edge) or active low (falling-edge)
 */
public enum ChipSelectMode {

    /**
     * CS is asserted low, the SPI transaction takes place, and then the signal is returned high.
     */
    SpiActiveLow,

    /**
     * CS is asserted high, the SPI transaction takes place, and then the signal is returned low.
     */
    SpiActiveHigh,

    /**
     * CS is pulsed high, and then the SPI transaction takes place.
     */
    PulseHighAtBeginning,

    /**
     * The SPI transaction takes place, and once finished, CS is pulsed high
     */
    PulseHighAtEnd,

    /**
     * CS is pulsed low, and then the SPI transaction takes place.
     */
    PulseLowAtBeginning
};
