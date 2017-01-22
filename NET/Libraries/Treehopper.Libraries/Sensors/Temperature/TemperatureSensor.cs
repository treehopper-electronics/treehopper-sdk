namespace Treehopper.Libraries.Sensors.Temperature
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Base temperature sensor functionality
    /// </summary>
    public abstract class TemperatureSensor : ITemperatureSensor
    {
        private double temperatureCelsius;

        public bool AutoUpdateWhenPropertyRead { get; set; } = true;

        /// <summary>
        /// Get the temperature, in Celsius
        /// </summary>
        public double Celsius
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    Update().Wait();

                return temperatureCelsius;
            }

            protected set
            {
                temperatureCelsius = value;
            }
        }

        /// <summary>
        /// Get the temperature, in Fahrenheit
        /// </summary>
        public double Fahrenheit
        {
            get
            {
                return ToFahrenheit(Celsius);
            }
        }

        /// <summary>
        /// Get the temperature, in Kelvin
        /// </summary>
        public double Kelvin
        {
            get
            {
                return ToKelvin(Celsius);
            }
        }

        public abstract Task Update();

        /// <summary>
        /// Utility method to convert any Celsius temperature to Kelvin
        /// </summary>
        /// <param name="temperatureCelsius">The original Celsius temperature</param>
        /// <returns>the Kelvin equivalent</returns>
        public static double ToKelvin(double temperatureCelsius)
        {
            return temperatureCelsius + 273.15;
        }

        /// <summary>
        /// Utility method to convert any Celsius temperature to Fahrenheit
        /// </summary>
        /// <param name="temperatureCelsius">The original Celsius temperature</param>
        /// <returns>the Fahrenheit equivalent</returns>
        public static double ToFahrenheit(double temperatureCelsius)
        {
            return ((temperatureCelsius * 9.0) / 5.0) + 32.0;
        }
    }
}
