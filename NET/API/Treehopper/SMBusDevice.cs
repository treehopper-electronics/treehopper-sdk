using System;
using System.Threading.Tasks;

namespace Treehopper
{
    /// <summary>
    ///     Device class used to abstract i2C interfacing
    /// </summary>
    public class SMBusDevice
    {
        /// <summary>
        ///     The address of the device
        /// </summary>
        private readonly byte address;

        /// <summary>
        ///     The I2c port used by the device
        /// </summary>
        private readonly I2c i2c;

        /// <summary>
        ///     The frequency to use
        /// </summary>
        private readonly int rateKhz;

        /// <summary>
        ///     Create a new SMBus device
        /// </summary>
        /// <param name="address">The 7-bit address of the device</param>
        /// <param name="i2cModule">The i2C module this device is connected to.</param>
        /// <param name="rateKHz">the rate, in kHz, that should be used to communicate with this device.</param>
        public SMBusDevice(byte address, I2c i2cModule, int rateKHz = 100)
        {
            if (address > 0x7f)
                throw new ArgumentOutOfRangeException("address",
                    "The address parameter expects a 7-bit address that doesn't include a Read/Write bit. The maximum address is 0x7F");
            this.address = address;
            i2c = i2cModule;
            rateKhz = rateKHz;
            i2c.Enabled = true;
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
        ///     Read a single byte from the device
        /// </summary>
        /// <returns></returns>
        public async Task<byte> ReadByte()
        {
            // set the speed for this device, just in case another device mucked with these settings
            i2c.Speed = rateKhz;

            // S Addr Rd [A] [Data] NA P
            var data = await i2c.SendReceive(address, new byte[] { }, 1).ConfigureAwait(false);
            return data[0];
        }

        /// <summary>
        ///     Write a single byte to the device
        /// </summary>
        /// <param name="data">the data to write</param>
        /// <returns></returns>
        public Task WriteByte(byte data)
        {
            // set the speed for this device, just in case another device mucked with these settings
            i2c.Speed = rateKhz;

            // S Addr Wr [A] Data [A] P
            return i2c.SendReceive(address, new[] {data}, 0);
        }

        /// <summary>
        ///     Write data directly to the device
        /// </summary>
        /// <param name="data">an array of bytes to write</param>
        /// <returns></returns>
        public Task WriteData(byte[] data)
        {
            // set the speed for this device, just in case another device mucked with these settings
            i2c.Speed = rateKhz;

            return i2c.SendReceive(address, data, 0);
        }

        /// <summary>
        ///     Read data directly from a device
        /// </summary>
        /// <param name="numBytesToRead">the number of bytes to read</param>
        /// <returns></returns>
        public Task<byte[]> ReadData(byte numBytesToRead)
        {
            // set the speed for this device, just in case another device mucked with these settings
            i2c.Speed = rateKhz;

            return i2c.SendReceive(address, null, numBytesToRead);
        }

        /// <summary>
        ///     Read an 8-bit register's value from the device
        /// </summary>
        /// <param name="register">the register address to read</param>
        /// <returns>the register's value as a byte</returns>
        public Task<byte> ReadByteData(int register)
        {
            // set the speed for this device, just in case another device mucked with these settings
            i2c.Speed = rateKhz;

            return ReadByteData((byte) register);
        }

        /// <summary>
        ///     Read an 8-bit register's value from the device
        /// </summary>
        /// <param name="register">the register address to read</param>
        /// <returns>the register's value as a byte</returns>
        public async Task<byte> ReadByteData(byte register)
        {
            // set the speed for this device, just in case another device mucked with these settings
            i2c.Speed = rateKhz;

            // S Addr Wr [A] Comm [A] S Addr Rd [A] [Data] NA P
            var data = await i2c.SendReceive(address, new[] {register}, 1).ConfigureAwait(false);
            return data[0];
        }

        /// <summary>
        ///     Read a 16-bit little-endian register value from the device
        /// </summary>
        /// <param name="register">the 8-bit register address to read from</param>
        /// <returns>the register's 16-bit value</returns>
        public async Task<ushort> ReadWordData(byte register)
        {
            // set the speed for this device, just in case another device mucked with these settings
            i2c.Speed = rateKhz;

            // S Addr Wr [A] Comm [A] S Addr Rd [A] [DataLow] A [DataHigh] NA P
            var result = await i2c.SendReceive(address, new[] {register}, 2).ConfigureAwait(false);
            return (ushort) ((result[1] << 8) | result[0]);
        }

        /// <summary>
        ///     Read a 16-bit big-endian register value from the device
        /// </summary>
        /// <param name="register">the 8-bit register address to read from</param>
        /// <returns>the register's 16-bit value</returns>
        public async Task<ushort> ReadWordDataBE(byte register)
        {
            // set the speed for this device, just in case another device mucked with these settings
            i2c.Speed = rateKhz;

            // S Addr Wr [A] Comm [A] S Addr Rd [A] [DataLow] A [DataHigh] NA P
            var result = await i2c.SendReceive(address, new[] {register}, 2).ConfigureAwait(false);
            return (ushort) ((result[0] << 8) | result[1]);
        }

        /// <summary>
        ///     Read a 16-bit little-endian value from the device
        /// </summary>
        /// <returns>the 16-bit value</returns>
        public async Task<ushort> ReadWord()
        {
            // set the speed for this device, just in case another device mucked with these settings
            i2c.Speed = rateKhz;

            // S Addr Wr [A] Comm [A] S Addr Rd [A] [DataLow] A [DataHigh] NA P
            var result = await i2c.SendReceive(address, null, 2).ConfigureAwait(false);
            return (ushort) ((result[1] << 8) | result[0]);
        }

        /// <summary>
        ///     Read a 16-bit little-endian value from the device
        /// </summary>
        /// <returns>the 16-bit value</returns>
        public async Task<ushort> ReadWordBE()
        {
            // set the speed for this device, just in case another device mucked with these settings
            i2c.Speed = rateKhz;

            // S Addr Wr [A] Comm [A] S Addr Rd [A] [DataHigh] A [DataLow] NA P
            var result = await i2c.SendReceive(address, null, 2).ConfigureAwait(false);
            return (ushort) ((result[0] << 8) | result[1]);
        }

        /// <summary>
        ///     Write a byte to a register
        /// </summary>
        /// <param name="register">the register to write the byte to</param>
        /// <param name="data">the byte to be written to the specified register</param>
        /// <returns>an awaitable task</returns>
        public Task WriteByteData(byte register, byte data)
        {
            // set the speed for this device, just in case another device mucked with these settings
            i2c.Speed = rateKhz;

            // S Addr Wr [A] Comm [A] Data [A] P
            return i2c.SendReceive(address, new[] {register, data}, 0);
        }

        /// <summary>
        ///     Write a 16-bit little-endian word to a register
        /// </summary>
        /// <param name="register">the register to write the 16-bit word to</param>
        /// <param name="data">the 16-bit word to write to the specified register</param>
        /// <returns>an awaitable task</returns>
        public Task WriteWordData(byte register, ushort data)
        {
            // set the speed for this device, just in case another device mucked with these settings
            i2c.Speed = rateKhz;

            // S Addr Wr [A] Comm [A] DataLow [A] DataHigh [A] P
            return i2c.SendReceive(address, new[] {register, (byte) (data & 0xFF), (byte) (data >> 8)}, 0);
        }

        /// <summary>
        ///     Write a 16-bit big-endian word to a register
        /// </summary>
        /// <param name="register">the register to write the 16-bit word to</param>
        /// <param name="data">the 16-bit word to write to the specified register</param>
        /// <returns>an awaitable task</returns>
        public Task WriteWordDataBE(byte register, ushort data)
        {
            // set the speed for this device, just in case another device mucked with these settings
            i2c.Speed = rateKhz;

            // S Addr Wr [A] Comm [A] DataHigh [A] DataLow [A] P
            return i2c.SendReceive(address, new[] {register, (byte) (data >> 8), (byte) (data & 0xFF)}, 0);
        }

        /// <summary>
        ///     Read one or more bytes from the specified register
        /// </summary>
        /// <param name="register">The register to read from</param>
        /// <param name="numBytes">The number of bytes to read</param>
        /// <returns>An awaitable array of bytes read</returns>
        public Task<byte[]> ReadBufferData(byte register, int numBytes)
        {
            // set the speed for this device, just in case another device mucked with these settings
            i2c.Speed = rateKhz;
            return i2c.SendReceive(address, new[] {register}, (byte) numBytes);
        }

        /// <summary>
        /// </summary>
        /// <param name="register"></param>
        /// <param name="buffer"></param>
        /// <returns>An awaitable task that completes upon success.</returns>
        public Task WriteBufferData(byte register, byte[] buffer)
        {
            // set the speed for this device, just in case another device mucked with these settings
            i2c.Speed = rateKhz;

            var data = new byte[buffer.Length + 1];
            data[0] = register;
            Array.Copy(buffer, 0, data, 1, buffer.Length);
            return i2c.SendReceive(address, data, 0);
        }
    }
}