using System;
using System.Collections.Generic;
using System.Text;

namespace Treehopper.Libraries.Sensors.Optical
{
    interface IAmbientLight : IPollable
    {
        double Lux { get; }
    }
}
