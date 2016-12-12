using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Treehopper.Libraries.Sensors.Inertial
{
    public interface IMagnetometer : IPollable
    {
        Vector3D Magnetometer { get; }
    }
}
