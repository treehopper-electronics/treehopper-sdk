using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Inertial
{
    /// <summary>
    /// Three-axis accelerometer sensor
    /// </summary>
    public interface IAccelerometer : IPollable
    {
        /// <summary>
        /// The three-axis acceleration reading, in g.
        /// </summary>
        Vector3 Accelerometer { get; }
    }
}
