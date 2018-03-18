using System.ComponentModel;

namespace Treehopper.Libraries.Sensors.Temperature
{
    /// <summary>
    ///     Temperature sensor interface
    /// </summary>
    public interface ITemperatureSensor : IPollable
    {
        /// <summary>
        ///     Celsius temperature
        /// </summary>
        double Celsius { get; }

        /// <summary>
        ///     Fahrenheit temperature
        /// </summary>
        double Fahrenheit { get; }

        /// <summary>
        ///     Kelvin temperature
        /// </summary>
        double Kelvin { get; }
    }
}