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
        int currentIndex = -1;

        /// <summary>
        /// Construct an I2cMux with a given muxed port
        /// </summary>
        /// <param name="UpstreamPort">The upstream port to mux</param>
        /// <param name="numPorts">The number of ports that belong to this mux</param>
        public I2cMux(I2c UpstreamPort, int numPorts)
        {
            this.UpstreamPort = UpstreamPort;
            UpstreamPort.Enabled = true;
            for (int i = 0; i < numPorts; i++)
                Ports.Add(new I2cMuxPort(this, i));
        }

        /// <summary>
        /// Collection of downstream I2C ports provided by this mux.
        /// </summary>
        public IList<I2cMuxPort> Ports { get; protected set; } = new List<I2cMuxPort>();

        /// <summary>
        /// The upstream I2C port that the selected downstream port should be connected to.
        /// </summary>
        public I2c UpstreamPort { get; set; }

        internal void SetMux(int index)
        {
            if (index == currentIndex) return;
            currentIndex = index;
            setMux(index);
        }

        /// <summary>
        /// Configure the mux to connect the specified <see cref="I2c"/> port to the upstream port 
        /// </summary>
        /// <param name="index">The index of the upstream port to connect</param>
        protected abstract void setMux(int index);
    }
}
