using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Libraries.Sensors;

namespace Treehopper.Libraries.Interface.PortExpander
{
    public interface IPortExpander<TPortExpanderPin> : IFlushableOutputPort<TPortExpanderPin> where TPortExpanderPin : IPortExpanderPin
    {
        IList<TPortExpanderPin> Pins { get; }
    }

    public interface IPortExpanderParent : IPollable
    {
        void OutputValueChanged(IPortExpanderPin portExpanderPin);
        void OutputModeChanged(IPortExpanderPin portExpanderPin);

        int AwaitPollingInterval { get; set; }
    }
}
