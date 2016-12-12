using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Temperature
{
    /// <summary>
    /// Base temperature sensor functionality
    /// </summary>
    public abstract class TemperatureSensor : Temperature, IPollable
    {
        public bool AutoUpdateWhenPropertyRead { get; set; } = true;

        /// <summary>
        /// Get the temperature, in Celsius
        /// </summary>
        public abstract double TemperatureCelsius { get; }

        /// <summary>
        /// Get the temperature, in Fahrenheit
        /// </summary>
        public double TemperatureFahrenheit
        {
            get
            {
                return TemperatureCelsius * 9.0 / 5.0 + 32.0;
            }
        }

        /// <summary>
        /// Get the temperature, in Kelvin
        /// </summary>
        public double TemperatureKelvin
        {
            get
            {
                return TemperatureCelsius + 273.15;
            }
        }

        public abstract Task Update();
    }
}
