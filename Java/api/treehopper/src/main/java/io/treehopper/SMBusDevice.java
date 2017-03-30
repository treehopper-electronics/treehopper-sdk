package io.treehopper;

import io.treehopper.interfaces.I2c;

/**
 * Device class used to abstract SMBus-compliant peripherals
 */
public class SMBusDevice {
    private I2c I2c;
    private byte address;
    int rateKhz;

    /**
     * Construct an SMBus peripheral with a communication rate of 100 kHz
     * @param address the address of the peripheral
     * @param I2CModule the I2c module to use
     */
    public SMBusDevice(byte address, I2c I2CModule) {
        this(address, I2CModule, 100);
    }

    /**
     * Construct an SMBus peripheral
     * @param address the address of the peripheral
     * @param I2cModule the I2c module to use
     * @param rateKHz the data rate, in kHz, to use
     */
    public SMBusDevice(byte address, I2c I2cModule, int rateKHz) {
        if (address > 0x7f)
            throw new IllegalArgumentException("The address parameter expects a 7-bit address that doesn't include a Read/Write bit. The maximum address is 0x7F");
        this.address = address;
        I2c = I2cModule;
        this.rateKhz = rateKHz;
        I2c.setEnabled(true);
    }
    // SMBus functions
    // Key to symbols
    // ==============

    // S     (1 bit) : Start bit
    // P     (1 bit) : Stop bit
    // Rd/Wr (1 bit) : Read/Write bit. Rd equals 1, Wr equals 0.
    // A, NA (1 bit) : Accept and reverse accept bit.
    // Addr  (7 bits): I2C 7 bit address. Note that this can be expanded as usual to
    //        get a 10 bit I2C address.
    // Comm  (8 bits): Command byte, a data byte which often selects a register on
    //        the device.
    // Data  (8 bits): A plain data byte. Sometimes, I write DataLow, DataHigh
    //        for 16 bit data.
    // Count (8 bits): A data byte containing the length of a block operation.

    // [..]: Data sent by I2C device, as opposed to data sent by the host adapter.

    /**
     * Read a single byte from the device
     * @return the byte read
     */
    public byte readByte() {
        I2c.setSpeed(rateKhz);

        // S Addr Rd [A] [Data] NA P
        byte[] data = I2c.sendReceive(this.address, new byte[]{}, 1);
        return data[0];
    }

    /**
     * Write a byte to the device
     * @param data the byte to write
     */
    public void writeByte(byte data) {
        I2c.setSpeed(rateKhz);

        // S Addr Wr [A] Data [A] P
        I2c.sendReceive(address, new byte[]{data}, 0);
    }

    /**
     * Read a little-endian 16-bit word from the device
     * @return the 16-bit value read from the device
     */
    public short readWord() {
        I2c.setSpeed(rateKhz);

        // S Addr Wr [A] Comm [A] S Addr Rd [A] [DataLow] A [DataHigh] NA P
        byte[] result = I2c.sendReceive(address, new byte[]{}, 2);
        return (short)(((0xff & result[1]) << 8) | (result[0] & 0xFF));
    }

    /**
     * Write a little-endian 16-bit word to the device
     * @param data the 16-bit value to write to the device
     */
    public void writeWord(short data) {
        I2c.setSpeed(rateKhz);

        // S Addr Wr [A] Comm [A] DataLow [A] DataHigh [A] P
        I2c.sendReceive(address, new byte[]{(byte) (data & 0xFF), (byte) ((data >> 8) & 0xFF) }, 0);
    }

    /**
     * Read a big-endian 16-bit word from the device
     * @return the 16-bit value read from the device
     */
    public short readWordBE() {
        I2c.setSpeed(rateKhz);

        // S Addr Wr [A] Comm [A] S Addr Rd [A] [DataLow] A [DataHigh] NA P
        byte[] result = I2c.sendReceive(address, new byte[]{}, 2);
        return (short)(((0xff & result[0]) << 8) | (result[1] & 0xFF));
    }

    /**
     * Write a big-endian 16-bit word to the device
     * @param data the 16-bit value to write to the device
     */
    public void writeWordBE(short data) {
        I2c.setSpeed(rateKhz);

        // S Addr Wr [A] Comm [A] DataLow [A] DataHigh [A] P
        I2c.sendReceive(address, new byte[]{(byte) ((data >> 8) & 0xFF), (byte) (data & 0xFF)}, 0);
    }

    /**
     * Read data directly from the device
     * @param numBytesToRead the number of bytes to read
     * @return the read data
     */
    public byte[] readData(byte numBytesToRead) {
        I2c.setSpeed(rateKhz);

        return I2c.sendReceive(address, null, numBytesToRead);
    }

    /**
     * Write data directly to the device
     * @param data the data to write to the device
     */
    public void writeData(byte[] data) {
        I2c.setSpeed(rateKhz);

        I2c.sendReceive(address, data, 0);
    }

    /**
     * Read a byte from the specified register
     * @param register the register to read from
     * @return the byte read
     */
    public byte readByteData(byte register) {
        I2c.setSpeed(rateKhz);

        // S Addr Wr [A] Comm [A] S Addr Rd [A] [Data] NA P
        byte[] data = I2c.sendReceive(this.address, new byte[]{register}, 1);
        return data[0];
    }

    /**
     * Write a byte to the specified register
     * @param register the register to write to
     * @param data the data to write
     */
    public void writeByteData(byte register, byte data) {
        I2c.setSpeed(rateKhz);

        // S Addr Wr [A] Comm [A] Data [A] P
        I2c.sendReceive(address, new byte[]{register, data}, 0);
    }

    /**
     * Read a little-endian 16-bit word from the given register
     * @param register the register to read from
     * @return the 16-bit word
     */
    public short readWordData(byte register) {
        I2c.setSpeed(rateKhz);

        // S Addr Wr [A] Comm [A] S Addr Rd [A] [DataLow] A [DataHigh] NA P
        byte[] result = I2c.sendReceive(address, new byte[]{register}, 2);
        return (short)(((0xff & result[1]) << 8) | (result[0] & 0xFF));
    }

    /**
     * Write a little-endian 16-bit word to the given register
     * @param register the register to write to
     * @param data the data to write
     */
    public void writeWordData(byte register, int data) {
        I2c.setSpeed(rateKhz);

        // S Addr Wr [A] Comm [A] DataLow [A] DataHigh [A] P
        I2c.sendReceive(address, new byte[]{register, (byte) (data & 0xFF), (byte) ((data >> 8) & 0xFF)}, 0);
    }

    /**
     * Read a big-endian 16-bit word from the given register
     * @param register the register to read from
     * @return the 16-bit word
     */
    public short readWordDataBE(byte register) {
        I2c.setSpeed(rateKhz);

        // S Addr Wr [A] Comm [A] S Addr Rd [A] [DataLow] A [DataHigh] NA P
        byte[] result = I2c.sendReceive(address, new byte[]{register}, 2);
        return (short)(((0xff & result[0]) << 8) | (result[1] & 0xFF));
    }

    /**
     * Write a big-endian 16-bit word to the given register
     * @param register the register to write the word to
     * @param data the data to write
     */
    public void writeWordDataBE(byte register, int data) {
        I2c.setSpeed(rateKhz);

        // S Addr Wr [A] Comm [A] DataLow [A] DataHigh [A] P
        I2c.sendReceive(address, new byte[]{register, (byte) ((data >> 8) & 0xFF), (byte) (data & 0xFF)}, 0);
    }

    /**
     * Read a byte array from the given register
     * @param register the register to read from
     * @param numBytes the number of bytes to read
     * @return the data read
     */
    public byte[] readBufferData(byte register, int numBytes) {
        I2c.setSpeed(rateKhz);

        return I2c.sendReceive(address, new byte[]{register}, (byte) numBytes);
    }

    /**
     * Write a byte array to the given register
     * @param register the register to write to
     * @param buffer the byte array to write
     */
    public void writeBufferData(byte register, byte[] buffer) {
        I2c.setSpeed(rateKhz);

        byte[] data = new byte[buffer.length + 1];
        System.arraycopy(buffer, 0, data, 1, buffer.length);
        data[0] = register;

        I2c.sendReceive(address, data, 0);
    }
}
