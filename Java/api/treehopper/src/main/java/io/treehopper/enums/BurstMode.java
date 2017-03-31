package io.treehopper.enums;

/**
 * SPI burst mode
 */
public enum BurstMode {

    /**
     * No burst -- always read the same number of bytes as transmitted
     */
    NoBurst,

    /**
     * Transmit burst -- don't return any data read from the bus
     */
    BurstTx,

    /**
     * receive burst -- ignore transmitted data above 53 bytes long, but receive the full number of bytes specified
     */
    BurstRx
}
