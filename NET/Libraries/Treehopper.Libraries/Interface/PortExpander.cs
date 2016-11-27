using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Libraries.Interface;

namespace Treehopper.Libraries.Interface
{
    public abstract class PortExpander : IFlushableOutputPort<PortExpanderPin>
    {
        public PortExpander(int numPins)
        {
            for (int i = 0; i < numPins; i++)
                Pins.Add(new PortExpanderPin(this, i));
        }
        public Collection<PortExpanderPin> Pins { get; set; } = new Collection<PortExpanderPin>();
        public bool AutoFlush { get; set; }

        public abstract Task Flush(bool force = false);

        public Task ReadAll()
        {
            return readPort();
        }

        internal void OutputValueChanged(PortExpanderPin portExpanderPin)
        {
            if (AutoFlush)
                outputValueChanged(portExpanderPin);
        }
        internal void OutputModeChanged(PortExpanderPin portExpanderPin)
        {
            if (AutoFlush)
                outputModeChanged(portExpanderPin);
        }
        protected abstract void outputValueChanged(PortExpanderPin portExpanderPin);

        protected abstract void outputModeChanged(PortExpanderPin portExpanderPin);

        protected abstract Task readPort();
    }

}
