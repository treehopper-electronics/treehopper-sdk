using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.PortExpander.AnalogMux
{
    class AnalogMux4051
    {
        public Collection<IAdcPin> AnalogPins { get; set; }
        public AnalogMux4051(IAdcPin adcInput, DigitalOutPin a, DigitalOutPin b, DigitalOutPin c)
        {

        }
    }
}
