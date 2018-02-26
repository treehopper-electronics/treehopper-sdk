using System;
using System.Collections.Generic;
using System.Text;

namespace Treehopper.Libraries.Sensors.Optical
{
    interface IAmbientLightSensor : IPollable
    {
        double Lux { get; }
    }
}
