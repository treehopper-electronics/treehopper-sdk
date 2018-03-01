using System.ComponentModel;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Temperature
{
    /// <summary>
    ///     Base temperature sensor functionality
    /// </summary>
    public abstract class TemperatureSensor : ITemperatureSensor
    {
        private double temperatureCelsius;

        public abstract event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Gets or sets whether this temperature sensor should be updated when read from
        /// </summary>
        public bool AutoUpdateWhenPropertyRead { get; set; } = true;

        /// <summary>
        ///     Get the temperature, in Celsius
        /// </summary>
        public double Celsius
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    UpdateAsync().Wait();

                return temperatureCelsius;
            }

            protected set { temperatureCelsius = value; }
        }

        /// <summary>
        ///     Get the temperature, in Fahrenheit
        /// </summary>
        public double Fahrenheit => ToFahrenheit(Celsius);

        /// <summary>
        ///     Get the temperature, in Kelvin
        /// </summary>
        public double Kelvin => ToKelvin(Celsius);

        /// <summary>
        ///     Update the temperature from the current value reported by the sensor
        /// </summary>
        /// <returns>An awaitable task</returns>
        public abstract Task UpdateAsync();

        /// <summary>
        ///     Utility method to convert any Celsius temperature to Kelvin
        /// </summary>
        /// <param name="temperatureCelsius">The original Celsius temperature</param>
        /// <returns>the Kelvin equivalent</returns>
        public static double ToKelvin(double temperatureCelsius)
        {
            return temperatureCelsius + 273.15;
        }

        /// <summary>
        ///     Utility method to convert any Celsius temperature to Fahrenheit
        /// </summary>
        /// <param name="temperatureCelsius">The original Celsius temperature</param>
        /// <returns>the Fahrenheit equivalent</returns>
        public static double ToFahrenheit(double temperatureCelsius)
        {
            return temperatureCelsius * 9.0 / 5.0 + 32.0;
        }

        /// <summary>
        ///     Returns a string representing the current temperature, in Celsius
        /// </summary>
        /// <returns>A string representing the current temperature</returns>
        public override string ToString()
        {
            return $"{Celsius:0.00} °C";
        }
    }
}