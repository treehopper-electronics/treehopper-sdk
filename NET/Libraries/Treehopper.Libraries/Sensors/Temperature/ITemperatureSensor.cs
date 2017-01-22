using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Temperature
{
    public interface ITemperatureSensor : IPollable
    {
        double Celsius { get; }

        double Fahrenheit { get; }

        double Kelvin { get; }
    }
}
