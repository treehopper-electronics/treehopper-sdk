using System.Threading.Tasks;

namespace Treehopper
{
    /// <summary>
    ///     Represents a USB connection for firmware uploading
    /// </summary>
    public interface IFirmwareConnection
    {
        /// <summary>
        ///     Open the first board currently in bootloader mode.
        /// </summary>
        /// <returns>True for successfully finding and opening a device. Otherwise, false.</returns>
        Task<bool> OpenAsync();

        /// <summary>
        ///     Close the firmware connection
        /// </summary>
        void Close();

        /// <summary>
        ///     Write data to the bootloader device
        /// </summary>
        /// <param name="data">A byte array of the data to write</param>
        /// <returns>An awaitable bool indicating whether the write operation was successful</returns>
        Task<bool> Write(byte[] data);

        /// <summary>
        ///     Read data from the bootloader device
        /// </summary>
        /// <param name="numBytes">The number of bytes to read</param>
        /// <returns>True if the read was successful; false otherwise.</returns>
        Task<byte[]> Read(int numBytes);
    }
}