using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Libraries.Interface;

namespace Treehopper.Libraries.Displays
{
    public interface LedDisplay : IFlushable
    {
        LedCollection Leds { get; }

        /// <summary>
        /// Write the LEDs without flushing the drivers
        /// </summary>
        void WriteLeds();
    }
}
