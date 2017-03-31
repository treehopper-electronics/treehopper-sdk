package io.treehopper.interfaces;

import java.util.List;

/**
 * One-wire bus
 */
public interface IOneWire {
    /**
     * Start OneWire mode on this host
     */
    void startOneWire();

    /**
     * Reset all devices on the OneWire bus and send the supplied address
     * @param address The address to address
     */
    void oneWireResetAndMatchAddress(long address);

    /**
     * Search all attached devices to discover addresses
     * @return a list of addresses of devices on the OneWire bus
     */
    List<Long> oneWireSearch();

    /**
     * Reset the OneWire bus to put all devices in a known state.
     * @return True if the reset was successful
     */
    boolean oneWireReset();

    /**
     * receive data from the OneWire bus
     * @param numBytes The number of bytes to receive
     * @return The bytes received
     */
    byte[] receive(int numBytes);

    /**
     * send an array of bytes to the OneWire bus
     * @param dataToSend  byte array of the data to send
     */
    void send(byte[] dataToSend);

    /**
     * send a single byte to the OneWire bus
     * @param data The byte to send
     */
    void send(byte data);
}
