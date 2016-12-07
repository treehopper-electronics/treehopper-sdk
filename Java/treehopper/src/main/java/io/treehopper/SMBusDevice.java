package io.treehopper;

import io.treehopper.interfaces.*;
/**
 * Created by jay on 12/7/2016.
 */
/// <summary>
/// Device class used to abstract i2C interfacing
/// </summary>
public class SMBusDevice {
    /// <summary>
    /// The I2c port used by the device
    /// </summary>
    protected I2c I2c;

    /// <summary>
    /// The address of the device
    /// </summary>
    protected byte address;

    /// <summary>
    /// The frequency to use
    /// </summary>
    int rateKhz;

    public SMBusDevice(byte address, I2c I2CModule) {
        this(address, I2CModule, 100);
    }
    /// <summary>
    /// Create a new SMBus device
    /// </summary>
    /// <param name="address">The 7-bit address of the device</param>
    /// <param name="I2CModule">The i2C module this device is connected to.</param>
    /// <param name="rateKHz">the rate, in kHz, that should be used to communicate with this device.</param>
    public SMBusDevice(byte address, I2c I2cModule, int rateKHz)
    {
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

    /// <summary>
    /// Read a single byte from the device
    /// </summary>
    /// <returns></returns>
    public byte ReadByte()
    {
        // set the speed for this device, just in case another device mucked with these settings
        I2c.setSpeed(rateKhz);

        // S Addr Rd [A] [Data] NA P
        byte[] data = I2c.sendReceive(this.address, new byte[] { }, 1);
        return data[0];
    }

    /// <summary>
    /// Write a single byte to the device
    /// </summary>
    /// <param name="data">the data to write</param>
    /// <returns></returns>
    public void WriteByte(byte data)
    {
        // set the speed for this device, just in case another device mucked with these settings
        I2c.setSpeed(rateKhz);

        // S Addr Wr [A] Data [A] P
        I2c.sendReceive(address, new byte[] { data }, 0);
    }

    /// <summary>
    /// Write data directly to the device
    /// </summary>
    /// <param name="data">an array of bytes to write</param>
    /// <returns></returns>
    public void WriteData(byte[] data)
    {
        // set the speed for this device, just in case another device mucked with these settings
        I2c.setSpeed(rateKhz);

        I2c.sendReceive(address, data, 0);
    }

    /// <summary>
    /// Read data directly from a device
    /// </summary>
    /// <param name="numBytesToRead">the number of bytes to read</param>
    /// <returns></returns>
    public byte[] ReadData(byte numBytesToRead)
    {
        // set the speed for this device, just in case another device mucked with these settings
        I2c.setSpeed(rateKhz);

        return I2c.sendReceive(address, null, numBytesToRead);
    }

    /// <summary>
    /// Read an 8-bit register's value from the device
    /// </summary>
    /// <param name="register">the register address to read</param>
    /// <returns>the register's value as a byte</returns>
    public byte ReadByteData(int register)
    {
        // set the speed for this device, just in case another device mucked with these settings
        I2c.setSpeed(rateKhz);

        return ReadByteData((byte)register);
    }
    /// <summary>
    /// Read an 8-bit register's value from the device
    /// </summary>
    /// <param name="register">the register address to read</param>
    /// <returns>the register's value as a byte</returns>
    public byte ReadByteData(byte register)
    {
        // set the speed for this device, just in case another device mucked with these settings
        I2c.setSpeed(rateKhz);

        // S Addr Wr [A] Comm [A] S Addr Rd [A] [Data] NA P
        byte[] data = I2c.sendReceive(this.address, new byte[] { register }, 1);
        return data[0];
    }

    /// <summary>
    /// Read a 16-bit register value from the device
    /// </summary>
    /// <param name="register">the 8-bit register address to read from</param>
    /// <returns>the register's 16-bit value</returns>
    public int ReadWordData(byte register)
    {
        // set the speed for this device, just in case another device mucked with these settings
        I2c.setSpeed(rateKhz);

        // S Addr Wr [A] Comm [A] S Addr Rd [A] [DataLow] A [DataHigh] NA P
        byte[] result = I2c.sendReceive(address, new byte[] { register }, 2);
        return (((int)(0xff & result[1]) << 8) | (int)(result[0] & 0xFF));
    }

    /// <summary>
    /// Write a byte to a register
    /// </summary>
    /// <param name="register">the register to write the byte to</param>
    /// <param name="data">the byte to be written to the specified register</param>
    /// <returns>an awaitable task</returns>
    public void WriteByteData(byte register, byte data)
    {
        // set the speed for this device, just in case another device mucked with these settings
        I2c.setSpeed(rateKhz);

        // S Addr Wr [A] Comm [A] Data [A] P
        I2c.sendReceive(address, new byte[] { register, data }, 0);
    }

    /// <summary>
    /// Write a 16-bit word to a register
    /// </summary>
    /// <param name="register">the register to write the 16-bit word to</param>
    /// <param name="data">the 16-bit word to write to the specified register</param>
    /// <returns>an awaitable task</returns>
    public void WriteWordData(byte register, int data)
    {
        // set the speed for this device, just in case another device mucked with these settings
        I2c.setSpeed(rateKhz);

        // S Addr Wr [A] Comm [A] DataLow [A] DataHigh [A] P
        I2c.sendReceive(address, new byte[] { register, (byte)(data & 0xFF), (byte)((data >> 8) & 0xFF) }, 0);
    }

    /// <summary>
    /// Read one or more bytes from the specified register
    /// </summary>
    /// <param name="register">The register to read from</param>
    /// <param name="numBytes">The number of bytes to read</param>
    /// <returns>An awaitable array of bytes read</returns>
    public byte[] ReadBufferData(byte register, int numBytes)
    {
        // set the speed for this device, just in case another device mucked with these settings
        I2c.setSpeed(rateKhz);

        return I2c.sendReceive(address, new byte[] { register }, (byte)numBytes);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="register"></param>
    /// <param name="buffer"></param>
    /// <returns>An awaitable task that completes upon success.</returns>
    public void WriteBufferData(byte register, byte[] buffer)
    {
        // set the speed for this device, just in case another device mucked with these settings
        I2c.setSpeed(rateKhz);

        byte[] data = new byte[buffer.length + 1];
        data[0] = register;
        System.arraycopy(buffer, 0, data, 1, buffer.length);
        I2c.sendReceive(address, data, 0);
    }
}
