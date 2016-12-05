using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface
{
    public interface IFlushable
    {
        bool AutoFlush { get; set; }

        /// <summary>
        /// Flush changed data to the port expander
        /// </summary>
        /// <param name="force">whether to flush *all* data to the port expander, even if it doesn't appear to have been changed</param>
        Task Flush(bool force = false);
    }
}
