using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Temperature
{
    public interface ITemperatureSensor : IPollable
    {
        double TemperatureCelsius { get; }
        double TemperatureFahrenheit { get; }
        double TemperatureKelvin { get; }
    }
}
