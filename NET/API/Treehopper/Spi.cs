namespace Treehopper
{
    using System.Threading.Tasks;

    /// <summary>
    /// An SPI interface
    /// </summary>
    public interface Spi
    {
        /// <summary>
        /// Enable or disable the SPI module.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Send/receive data
        /// </summary>
        /// <param name="dataToWrite">a byte array of the data to send. The length of the transaction is determined by the length of this array.</param>
        /// <param name="chipSelect">The chip select pin, if any, to use during this transaction.</param>
        /// <param name="chipSelectMode">The chip select mode to use during this transaction (if a CS pin is selected)</param>
        /// <param name="speedMhz">The speed to perform this transaction at.</param>
        /// <param name="burstMode">Whether to use one of the burst modes</param>
        /// <param name="spiMode">The SPI mode to use during this transaction.</param>
        /// <returns>An awaitable byte array with the received data.</returns>
        Task<byte[]> SendReceive(
            byte[] dataToWrite, 
            SpiChipSelectPin chipSelect = null, 
            ChipSelectMode chipSelectMode = ChipSelectMode.SpiActiveLow, 
            double speedMhz = 1, 
            SpiBurstMode burstMode = SpiBurstMode.NoBurst, 
            SpiMode spiMode = SpiMode.Mode00
        );
    }
}
