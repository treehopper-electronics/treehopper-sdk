using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    public interface SpiChipSelectPin : DigitalOutPin
    {
        int PinNumber { get; }

        Spi SpiModule { get; }

    }
}
