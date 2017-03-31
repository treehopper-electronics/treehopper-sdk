package io.treehopper.enums;

/**
 * UART mode
 */
public enum UartMode
{
    /**
     * The module is operating in UART mode
     */
    Uart,

    /**
     * The module is operating in OneWire mode. Only the TX pin is used.
     */
    OneWire
}