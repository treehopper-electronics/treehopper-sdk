using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Temperature
{
    public class TemperatureSensor : ITemperature
    {
        public virtual double TemperatureCelsius { get; protected set; }

        public double TemperatureFahrenheit
        {
            get
            {
                return TemperatureCelsius * 9.0 / 5.0 + 32.0;
            }
        }

        public double TemperatureKelvin
        {
            get
            {
                return TemperatureCelsius + 273.15;
            }
        }
    }
}
