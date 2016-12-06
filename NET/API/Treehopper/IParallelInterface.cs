using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    /// <summary>
    /// Represents an 8080-style parallel interface that can write data
    /// </summary>
    public interface WriteOnlyParallelInterface
    {
        /// <summary>
        /// Gets or sets whether the bus is enabled
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// The minimum number of microseconds to delay between each word write. 
        /// </summary>
        int DelayMicroseconds { get; set; }

        /// <summary>
        /// Gets the width of the bus
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Write one or more bytes to the command register
        /// </summary>
        /// <param name="command">The command data to write</param>
        /// <returns>An awaitable task that completes when the write operation finishes</returns>
        Task WriteCommand(uint[] command);

        /// <summary>
        /// Write one or more bytes to the data register
        /// </summary>
        /// <param name="data">The data to write</param>
        /// <returns>An awaitable task that completes when the write operation finishes</returns>
        Task WriteData(uint[] data);

    }
    /// <summary>
    /// A parallel interface that can read and write data
    /// </summary>
    public interface ReadWriteParallelInterface : WriteOnlyParallelInterface
    {
        /// <summary>
        /// Read one or more words from the command register
        /// </summary>
        /// <param name="command">The command to write before reading</param>
        /// <param name="length">The number of words to read</param>
        /// <returns>The words read</returns>
        Task<ushort[]> ReadCommand(uint command, int length);

        /// <summary>
        /// Read one or more words from the data register
        /// </summary>
        /// <param name="length">The number of words to read</param>
        /// <returns>The words read</returns>
        Task<ushort[]> ReadData(int length);

    }
}
