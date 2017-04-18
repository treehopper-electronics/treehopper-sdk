using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface.PortExpander
{
    /// <summary>
    /// Base class representing an I/O port expander
    /// </summary>
    public abstract class PortExpander : IPortExpander<PortExpanderPin>, IPortExpanderParent
    {
        /// <summary>
        /// Construct a port expander with the supplied number of pins
        /// </summary>
        /// <param name="numPins">the number of pins of this port expander</param>
        public PortExpander(int numPins)
        {
            for (int i = 0; i < numPins; i++)
                Pins.Add(new PortExpanderPin(this, i));
        }

        /// <summary>
        /// The collection of pins that belong to this port expander
        /// </summary>
        public IList<PortExpanderPin> Pins { get; set; } = new Collection<PortExpanderPin>();

        /// <summary>
        /// The parent device. Always returns null.
        /// </summary>
        public IFlushable Parent => null;

        /// <summary>
        /// Whether this port expander should auto-flush
        /// </summary>
        public bool AutoFlush { get; set; }

        /// <summary>
        /// Whether to read the port expander when a pin is read from
        /// </summary>
        public bool AutoUpdateWhenPropertyRead { get; set; } = true;

        /// <summary>
        /// The polling interval to use for AwaitDigitalValueChanged() calls
        /// </summary>
        public int AwaitPollingInterval { get; set; }

        /// <summary>
        /// Flush output data to the port
        /// </summary>
        /// <param name="force"></param>
        /// <returns>An awaitable task that completes when the flush finishes.</returns>
        public abstract Task Flush(bool force = false);

        /// <summary>
        /// Called whenever an output changes
        /// </summary>
        /// <param name="portExpanderPin">The pin whose output changed</param>
        protected abstract Task outputValueChanged(IPortExpanderPin portExpanderPin);

        /// <summary>
        /// Called whenever the mode of a pin changes
        /// </summary>
        /// <param name="portExpanderPin">The pin who has a new mode</param>
        protected abstract Task outputModeChanged(IPortExpanderPin portExpanderPin);

        /// <summary>
        /// Called whenever a read is requested from the bus of the current input values
        /// </summary>
        /// <returns>An awaitable task that completes when the read is finished</returns>
        protected abstract Task readPort();

        Task IPortExpanderParent.OutputValueChanged(IPortExpanderPin portExpanderPin)
        {
            return outputValueChanged(portExpanderPin);
        }

        Task IPortExpanderParent.OutputModeChanged(IPortExpanderPin portExpanderPin)
        {
            return outputModeChanged(portExpanderPin);
        }

        /// <summary>
        /// Read the port's value
        /// </summary>
        /// <returns>An awaitable task</returns>
        public Task Update()
        {
            return readPort();
        }
    }

}
