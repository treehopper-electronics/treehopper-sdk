using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Treehopper.Libraries.Sensors.Inertial
{
    public interface IGyroscope : IPollable
    {
        Vector3D Gyroscope { get; }
    }
}
