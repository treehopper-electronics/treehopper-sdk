using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    /// <summary>
    /// Base interface representing pins capable of reading analog values
    /// </summary>
    public interface AdcPin
    {
        /// <summary>
        /// Make this pin an ADC pin
        /// </summary>
        void MakeAnalogIn();
    }
}
