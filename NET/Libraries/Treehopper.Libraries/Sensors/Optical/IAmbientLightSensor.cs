using System;
using System.Collections.Generic;
using System.Text;

namespace Treehopper.Libraries.Sensors.Optical
{
    /// <summary>
    /// Ambient light sensor interface
    /// </summary>
    public interface IAmbientLightSensor : IPollable
    {
        /// <summary>
        /// Gets the ambient light sensor reading, in lux
        /// </summary>
        double Lux { get; }
    }
}
