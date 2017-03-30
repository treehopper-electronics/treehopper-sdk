package io.treehopper.enums;

/**
 * Defines the clock phase and polarity used by the SPI peripheral
 */
public enum SpiMode {
    /**
     * Clock is initially low; data is valid on the rising edge of the clock
     */
    Mode00((byte) 0x00),

    /**
     * Clock is initially low; data is valid on the falling edge of the clock
     */
    Mode01((byte) 0x20),

    /**
     * Clock is initially high; data is valid on the rising edge of the clock
     */
    Mode10((byte) 0x10),

    /**
     * Clock is initially high; data is valid on the falling edge of the clock
     */
    Mode11((byte) 0x30);

    private final byte modeRegisterValue;

    SpiMode(byte modeRegisterValue) {
        this.modeRegisterValue = modeRegisterValue;
    }

    public byte getModeRegisterValue() {
        return modeRegisterValue;
    }
}