using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Inertial
{
    /// <summary>
    /// Three-axis magnetometer (digital compass)
    /// </summary>
    public interface IMagnetometer : IPollable
    {
        /// <summary>
        /// Gets the three-axis magnetometer value
        /// </summary>
        Vector3 Magnetometer { get; }
    }
}
