using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Inertial
{
    /// <summary>
    /// Three-axis gyroscope sensor
    /// </summary>
    public interface IGyroscope : IPollable
    {
        /// <summary>
        /// The three-axis gyroscope data
        /// </summary>
        Vector3 Gyroscope { get; }
    }
}
