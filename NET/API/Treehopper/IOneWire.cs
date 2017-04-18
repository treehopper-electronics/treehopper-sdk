using System.Collections.Generic;
using System.Threading.Tasks;

namespace Treehopper
{
    /// <summary>
    ///     Represents a OneWire-capable host.
    /// </summary>
    public interface IOneWire
    {
        /// <summary>
        ///     Start OneWire mode on this host
        /// </summary>
        void StartOneWire();

        /// <summary>
        ///     Reset all devices on the OneWire bus and send the supplied address
        /// </summary>
        /// <param name="address">The address to address</param>
        /// <returns>An awaitable Task that completes upon success.</returns>
        Task OneWireResetAndMatchAddress(ulong address);

        /// <summary>
        ///     Search all attached devices to discover addresses
        /// </summary>
        /// <returns>A list of addresses of devices on the OneWire bus</returns>
        Task<List<ulong>> OneWireSearch();

        /// <summary>
        ///     Reset the OneWire bus to put all devices in a known state.
        /// </summary>
        /// <returns>True if the reset was successful</returns>
        Task<bool> OneWireReset();

        /// <summary>
        ///     Receive data from the OneWire bus
        /// </summary>
        /// <param name="numBytes">The number of bytes to receive</param>
        /// <returns>The bytes received</returns>
        Task<byte[]> Receive(int numBytes);

        /// <summary>
        ///     Send an array of bytes to the OneWire bus
        /// </summary>
        /// <param name="dataToSend">A byte array of the data to send</param>
        /// <returns>An awaitable task that completes when the send is finished.</returns>
        Task Send(byte[] dataToSend);

        /// <summary>
        ///     Send a single byte to the OneWire bus
        /// </summary>
        /// <param name="data">The byte to send</param>
        /// <returns>An awaitable task that completes when the send is finished.</returns>
        Task Send(byte data);
    }
}