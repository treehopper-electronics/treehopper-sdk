namespace Treehopper.Libraries.Interface.Dac
{
    /// <summary>
    ///     Represents a single DAC channel
    /// </summary>
    public interface Dac
    {
        /// <summary>
        ///     The value the DAC channel is outputting
        /// </summary>
        double Value { get; set; }

        /// <summary>
        ///     The voltage the DAC channel is outputting
        /// </summary>
        double Voltage { get; set; }

        /// <summary>
        ///     The actual bit value the DAC channel is outputting
        /// </summary>
        int DacValue { get; set; }
    }
}