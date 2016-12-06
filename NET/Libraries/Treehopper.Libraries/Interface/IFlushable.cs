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
    }
}
