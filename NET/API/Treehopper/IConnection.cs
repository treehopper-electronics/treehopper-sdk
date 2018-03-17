using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Treehopper
{
    /// <summary>
    ///     Pin event data delegate
    /// </summary>
    /// <param name="data">The pin event data sent by the board</param>
    public delegate void PinEventData(byte[] data);

    /// <summary>
    ///     A delegate representing a new connection
    /// </summary>
    /// <param name="connectionAdded">The new connection</param>
    public delegate void ConnectionAddedHandler(IConnection connectionAdded);

    /// <summary>
    ///     A delegate representing a removed connection
    /// </summary>
    /// <param name="connectionRemoved">The old connection</param>
    public delegate void ConnectionRemovedHandler(IConnection connectionRemoved);

    /// <summary>
    ///     UsbConnection interface
    /// </summary>
    public interface IConnection : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        ///     Get the serial number of the device
        /// </summary>
        string Serial { get; }

        /// <summary>
        ///     Get the name of the device
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Get the version number of the device
        /// </summary>
        ushort Version { get; }

        /// <summary>
        ///     Get the system path of the device
        /// </summary>
        string DevicePath { get; }

        /// <summary>
        ///     Gets or sets the update rate of the board
        /// </summary>
        int UpdateRate { get; set; }

        /// <summary>
        ///     Fires whenever pin event data is received
        /// </summary>
        event PinEventData PinEventDataReceived;

        /// <summary>
        ///     Close the connection
        /// </summary>
        void Close();

        /// <summary>
        ///     Open the connection
        /// </summary>
        /// <returns></returns>
        Task<bool> OpenAsync();

        /// <summary>
        ///     Send pin configuration data
        /// </summary>
        /// <param name="data">The data to send</param>
        Task SendDataPinConfigChannelAsync(byte[] data);

        /// <summary>
        ///     Send peripheral configuration data
        /// </summary>
        /// <param name="data">The data to send</param>
        Task SendDataPeripheralChannelAsync(byte[] data);

        /// <summary>
        ///     Read peripheral data from the board
        /// </summary>
        /// <param name="numBytesToRead">The number of bytes to read</param>
        /// <returns>The bytes read</returns>
        Task<byte[]> ReadPeripheralResponsePacketAsync(uint numBytesToRead);
    }
}