using System.Threading.Tasks;

namespace Treehopper
{
    /// <summary>
    /// I2c master device
    /// </summary>
/**
This interface is used as a contract for I2C devices. 
        
For documentation on %Treehopper's %I2C peripheral, consult HardwareI2C
*/
    public interface I2C
    {
        /// <summary>
        ///     The speed, in KHz, that the bus should be operated at. The default is 100 KHz for most modules.
        /// </summary>
        double Speed { get; set; }

        /// <summary>
        ///     Whether this I2c port should be enabled or disabled
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        ///     Perform an I2c transaction to a peripheral slave device using this port.
        /// </summary>
        /// <param name="address">The 7-bit address of the slave. Valid range is 0-127.</param>
        /// <param name="dataToWrite">A byte array of the data to write to the device.</param>
        /// <param name="numBytesToRead">The number of bytes to read and return.</param>
        /// <returns>The bytes read from the device.</returns>
        Task<byte[]> SendReceiveAsync(byte address, byte[] dataToWrite, byte numBytesToRead);
    }
}