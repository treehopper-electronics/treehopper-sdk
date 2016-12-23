using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Inertial
{
    public interface IAccelerometer : IPollable
    {
        Vector3 Accelerometer { get; }
    }
}
