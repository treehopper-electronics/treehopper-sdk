using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface.Dac
{
    public interface IDac
    {
        double Value { get; set; }

        double Voltage { get; set; }

        int DacValue { get; set; }
    }
}
