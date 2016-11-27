using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface.I2cMux
{
    /// <summary>
    /// Base class for any circuit that implements an i2C mux
    /// </summary>
    public abstract class I2cMux
    {
        /// <summary>
        /// Collection of downstream I2C ports provided by this mux.
        /// </summary>
        public Collection<I2cMuxPort> Ports { get; protected set; }

        /// <summary>
        /// The upstream I2C port that the selected downstream port should be connected to.
        /// </summary>
        public II2c UpstreamPort { get; set; }

        /// <summary>
        /// Construct an I2cMux with a given muxed port
        /// </summary>
        /// <param name="UpstreamPort">The upstream port to mux</param>
        public I2cMux(II2c UpstreamPort)
        {
            this.UpstreamPort = UpstreamPort;
        }

        protected abstract void setMux(int index);
    }
}
