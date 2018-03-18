using System;
using System.Collections.Generic;
using System.Text;

namespace Treehopper.Libraries.Sensors
{
    interface IProximity : IPollable
    {
        /// <summary>
        /// Get the proximity reported by the sensor, in centimeters (cm)
        /// </summary>
        double Centimeters { get; }

        /// <summary>
        /// Get the proximity reported by the sensor, in inches (in)
        /// </summary>
        double Inches { get; }

        /// <summary>
        /// Get the proximity reported by the sensor, in feet (ft)
        /// </summary>
        double Feet { get; }

        /// <summary>
        /// Get the proximity reported by the sensor, in meters (m)
        /// </summary>
        double Meters { get; }
    }
}
