namespace Treehopper.Libraries.Sensors.Temperature
{
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
        public double TemperatureCelsius
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
        public double TemperatureFahrenheit
        {
            get
            {
                return ((TemperatureCelsius * 9.0) / 5.0) + 32.0;
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
