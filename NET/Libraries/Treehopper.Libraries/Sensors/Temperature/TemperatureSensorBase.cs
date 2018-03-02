using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Temperature
{
    /// <summary>
    ///     Base temperature sensor functionality
    /// </summary>
    public abstract class TemperatureSensorBase : ITemperatureSensor
    {
        protected double celsius;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets whether reading from the sensor's properties should request updates from the sensor automatically (defaults to true).
        /// </summary>
        /// <remarks>
        /// By default, whenever you access one of the properties of this sensor, a new reading will be fetched. If this property
        /// is set to false, you must manually call the UpdateAsync() method to retrieve a new sensor reading.
        /// </remarks>
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

                return celsius;
            }
        }

        /// <summary>
        ///     Get the temperature, in Fahrenheit
        /// </summary>
        public double Fahrenheit
        {
            get
            {
                return ToFahrenheit(Celsius);
            }
        }

        /// <summary>
        ///     Get the temperature, in Kelvin
        /// </summary>
        public double Kelvin
        {
            get
            {
                return ToKelvin(Celsius);
            }
        }

        /// <summary>
        /// Requests a reading from the sensor and updates its data properties with the gathered values.
        /// </summary>
        /// <returns>An awaitable Task</returns>
        /// <remarks>
        /// Note that when #AutoUpdateWhenPropertyRead is `true` (which it is, by default), this method is implicitly 
        /// called when any sensor data property is read from --- there's no need to call this method unless you set
        /// AutoUpdateWhenPropertyRead to `false`.
        /// 
        /// Unless otherwise noted, this method updates all sensor data simultaneously, which can often lead to more efficient
        /// bus usage (as well as reducing USB chattiness).
        /// </remarks>
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

        protected void RaisePropertyChanged(object sender)
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(nameof(Celsius)));
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(nameof(Fahrenheit)));
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(nameof(Kelvin)));
        }

        protected void RaisePropertyChanged(object sender, string property)
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(property));
        }
    }
}