using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface
{
    /// <summary>
    /// Represents any object that has a flushable interface
    /// </summary>
    public interface IFlushable
    {
        /// <summary>
        /// Whether this interface should automatically flush new values or not
        /// </summary>
        bool AutoFlush { get; set; }

        /// <summary>
        /// Flush changed data to the port expander
        /// </summary>
        /// <param name="force">whether to flush *all* data to the port expander, even if it doesn't appear to have been changed</param>
        Task Flush(bool force = false);

        /// <summary>
        /// Gets or sets the parent flushable device (if it exists); if this property is set by this driver, it is expected that flushing the parent will also flush this device
        /// </summary>
        /// <remarks>
        /// <para>This property is designed to make LED displays, which operate across groups of LEDs (and possibly groups of LED drivers), much more efficient to update. Many commonly-used LED drivers are shift registers that are chained together; since these cannot be individually addressed, any write to one must include a write to all the other ones. By properly setting the parent shift register in each chain, displays can optimize these updates. </para>
        /// </remarks>
        IFlushable Parent { get; }
    }
}
