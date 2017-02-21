using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Temperature
{
    /// <summary>
    /// Temperature sensor interface
    /// </summary>
    public interface ITemperatureSensor : IPollable
    {
        /// <summary>
        /// Celsius temperature
        /// </summary>
        double Celsius { get; }

        /// <summary>
        /// Fahrenheit temperature
        /// </summary>
        double Fahrenheit { get; }

        /// <summary>
        /// Kelvin temperature
        /// </summary>
        double Kelvin { get; }
    }
}
