using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface
{
    public interface IFlushableOutputPort<TDigitalPin> : IFlushable where TDigitalPin : DigitalOutPin
    //public interface IFlushableOutputPort
    {
        Collection<TDigitalPin> Pins { get; }
    }
}
