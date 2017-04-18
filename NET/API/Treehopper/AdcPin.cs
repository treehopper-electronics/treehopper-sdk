using System.Threading.Tasks;

namespace Treehopper
{
    /// <summary>
    ///     Base interface representing pins capable of reading analog values
    /// </summary>
    public interface AdcPin
    {
        /// <summary>
        ///     Retrieve the last value obtained from the ADC.
        /// </summary>
        /// <remarks>
        ///     Treehopper has a 12-bit ADC, so ADC values will range from 0-4095.
        /// </remarks>
        int AdcValue { get; }

        /// <summary>
        ///     Retrieve the last voltage reading from the ADC.
        /// </summary>
        double AnalogVoltage { get; }

        /// <summary>
        ///     Retrieve the last reading from the ADC, expressed on a unit range (0.0 - 1.0)
        /// </summary>
        /// <remarks>
        /// </remarks>
        double AnalogValue { get; }

        /// <summary>
        ///     Gets or sets the voltage threshold required to fire the AnalogVoltageChanged event.
        /// </summary>
        double AnalogVoltageChangedThreshold { get; set; }

        /// <summary>
        ///     Gets or sets the value threshold required to fire the AdcValueChanged event.
        /// </summary>
        int AdcValueChangedThreshold { get; set; }

        /// <summary>
        ///     Gets or sets the value threshold required to fire the AnalogValueChanged event.
        /// </summary>
        double AnalogValueChangedThreshold { get; set; }

        /// <summary>
        ///     Occurs when an analog voltage is changed, according to the set threshold.
        /// </summary>
        /// <remarks>
        ///     The Changed event is raised when the 12-bit ADC value obtained is different from the previous reading
        ///     by at least the value specified by <see cref="AnalogVoltageChangedThreshold" />.
        /// </remarks>
        event OnAnalogVoltageChanged AnalogVoltageChanged;

        /// <summary>
        ///     Occurs when an analog value is changed, according to the set threshold.
        /// </summary>
        /// <remarks>
        ///     The Changed event is raised when the 10-bit ADC value obtained is different from the previous reading
        ///     by at least the value specified by <see cref="AdcValueChangedThreshold" />
        /// </remarks>
        event OnAnalogValueChanged AnalogValueChanged;

        /// <summary>
        ///     Occurs when the normalized analog value is changed, according to the set threshold.
        /// </summary>
        /// <remarks>
        ///     The Changed event is raised when the value obtained is different from the previous reading.
        /// </remarks>
        event OnAdcValueChanged AdcValueChanged;

        /// <summary>
        ///     Make this pin an ADC pin
        /// </summary>
        Task MakeAnalogIn();
    }
}