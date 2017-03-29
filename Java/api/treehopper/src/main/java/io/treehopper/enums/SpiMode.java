package io.treehopper.enums;

/**
 * Created by jay on 12/6/2016.
 */

/// <summary>
/// Defines the clock phase and polarity used for SPI communication
/// </summary>
/// <remarks>
/// <para>The left number indicates the clock polarity (CPOL), while the right number indicates the clock phase (CPHA). Consult https://en.wikipedia.org/wiki/Serial_Peripheral_Interface_Bus#Clock_polarity_and_phase for more information.</para>
/// <para>Note that the numeric values of this enum do not match the standard nomenclature, but instead match the value needed by Treehopper's MCU. Do not attempt to cast integers from/to this enum.</para>
/// </remarks>
public enum SpiMode
{
    /// <summary>
    /// Clock is initially low; data is valid on the rising edge of the clock
    /// </summary>
    Mode00((byte)0x00),

    /// <summary>
    /// Clock is initially low; data is valid on the falling edge of the clock
    /// </summary>

    Mode01((byte)0x20),

    /// <summary>
    /// Clock is initially high; data is valid on the rising edge of the clock
    /// </summary>
    Mode10((byte)0x10),

    /// <summary>
    /// Clock is initially high; data is valid on the falling edge of the clock
    /// </summary>
    Mode11((byte)0x30);

    SpiMode(byte modeRegisterValue) {
        this.modeRegisterValue = modeRegisterValue;
    }
    private final byte modeRegisterValue;

    public byte getModeRegisterValue() {
        return modeRegisterValue;
    }
}