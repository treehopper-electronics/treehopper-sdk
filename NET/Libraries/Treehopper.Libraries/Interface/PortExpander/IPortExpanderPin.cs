using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface.PortExpander
{
    /// <summary>
    /// A port expander pin
    /// </summary>
    public interface IPortExpanderPin : DigitalIOPin
    {
        /// <summary>
        /// The mode to use with this port expander pin
        /// </summary>
        PortExpanderPinMode Mode { get; set; }

        /// <summary>
        /// The pin (bit) number of this port expander pin
        /// </summary>
        int PinNumber { get; }

    }
}
