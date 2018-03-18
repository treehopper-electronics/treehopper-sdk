using System.Collections.Generic;

namespace Treehopper.Libraries.IO.Mux
{
    /// <summary>
    ///     Base class for any circuit that implements an i2C mux
    /// </summary>
    public abstract class I2cMux
    {
        private int currentIndex = -1;

        /// <summary>
        ///     Construct an I2cMux with a given muxed port
        /// </summary>
        /// <param name="UpstreamPort">The upstream port to mux</param>
        /// <param name="numPorts">The number of ports that belong to this mux</param>
        public I2cMux(I2C UpstreamPort, int numPorts)
        {
            this.UpstreamPort = UpstreamPort;
            UpstreamPort.Enabled = true;
            for (var i = 0; i < numPorts; i++)
                Ports.Add(new I2cMuxPort(this, i));
        }

        /// <summary>
        ///     Collection of downstream I2C ports provided by this mux.
        /// </summary>
        public IList<I2cMuxPort> Ports { get; protected set; } = new List<I2cMuxPort>();

        /// <summary>
        ///     The upstream I2C port that the selected downstream port should be connected to.
        /// </summary>
        public I2C UpstreamPort { get; set; }

        internal void SetMux(int index)
        {
            if (index == currentIndex) return;
            currentIndex = index;
            setMux(index);
        }

        /// <summary>
        ///     Configure the mux to connect the specified <see cref="I2C" /> port to the upstream port
        /// </summary>
        /// <param name="index">The index of the upstream port to connect</param>
        protected abstract void setMux(int index);
    }
}