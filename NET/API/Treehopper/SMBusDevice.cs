using System;
using System.Threading.Tasks;
using Treehopper;
namespace Treehopper.Libraries
{
    public class SMBusDevice
    {
        protected II2c I2c;
        protected byte address;
        public SMBusDevice(byte address, II2c I2CModule, int rateKHz = 100)
        {
            this.address = address;
            I2c = I2CModule;
            //I2c.Start(rateKHz);
            I2c.Enabled = true;
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
        public async Task<byte> ReadByte()
        {
            // S Addr Rd [A] [Data] NA P
            byte[] data = await I2c.SendReceive(this.address, new byte[] { }, 1);
            return data[0];
        }

        /// <summary>
        /// Write a single byte to the device
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task WriteByte(byte data)
        {
            // S Addr Wr [A] Data [A] P
            await I2c.SendReceive(address, new byte[] { data }, 0);
        }

        /// <summary>
        /// Read an 8-bit register's value from the device
        /// </summary>
        /// <param name="register">the register address to read</param>
        /// <returns>the register's value as a byte</returns>
        public Task<byte> ReadByteData(int register)
        {
            return ReadByteData((byte)register);
        }
        /// <summary>
        /// Read an 8-bit register's value from the device
        /// </summary>
        /// <param name="register">the register address to read</param>
        /// <returns>the register's value as a byte</returns>
        public async Task<byte> ReadByteData(byte register)
        {
            // S Addr Wr [A] Comm [A] S Addr Rd [A] [Data] NA P
            byte[] data = await I2c.SendReceive(this.address, new byte[] { register }, 1);
            return data[0];
        }

        /// <summary>
        /// Read a 16-bit register value from the device
        /// </summary>
        /// <param name="register">the 8-bit register address to read from</param>
        /// <returns>the register's 16-bit value</returns>
        public async Task<UInt16> ReadWordData(byte register)
        {
            // S Addr Wr [A] Comm [A] S Addr Rd [A] [DataLow] A [DataHigh] NA P
           byte[] result = await I2c.SendReceive(address, new byte[] { register }, 2);
            return (UInt16)((result[1] << 8) | result[0]);
        }

        /// <summary>
        /// Write a byte to a register
        /// </summary>
        /// <param name="register">the register to write the byte to</param>
        /// <param name="data">the byte to be written to the specified register</param>
        /// <returns>an awaitable task</returns>
        public async Task WriteByteData(byte register, byte data)
        {
            // S Addr Wr [A] Comm [A] Data [A] P
            await I2c.SendReceive(address, new byte[] { register, data }, 0);
        }

        /// <summary>
        /// Write a 16-bit word to a register
        /// </summary>
        /// <param name="register">the register to write the 16-bit word to</param>
        /// <param name="data">the 16-bit word to write to the specified register</param>
        /// <returns>an awaitable task</returns>
        public async Task WriteWordData(byte register, UInt16 data)
        {
            // S Addr Wr [A] Comm [A] DataLow [A] DataHigh [A] P
            await I2c.SendReceive(address, new byte[] { register, (byte)(data & 0xFF), (byte)(data << 8) }, 0);
        }
    }
}
