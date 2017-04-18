using System.Threading.Tasks;

namespace Treehopper
{
    /// <summary>
    ///     Describes an I2c master device.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         To support the widest variety of hardware configurations, peripheral libraries should always use this class
    ///         instead of <see cref="HardwareI2c" />.
    ///     </para>
    /// </remarks>
    public interface I2c
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
        Task<byte[]> SendReceive(byte address, byte[] dataToWrite, byte numBytesToRead);
    }
}