namespace Treehopper
{
    /// <summary>
    /// Defines whether a signal is active high (rising-edge) or active low (falling-edge)
    /// </summary>
    public enum ChipSelectMode
    {
        /// <summary>
        /// CS is asserted low, the SPI transaction takes place, and then the signal is returned high.
        /// </summary>
        SpiActiveLow,

        /// <summary>
        /// CS is asserted high, the SPI transaction takes place, and then the signal is returned low.
        /// </summary>
        SpiActiveHigh,

        /// <summary>
        /// CS is pulsed high, and then the SPI transaction takes place.
        /// </summary>
        PulseHighAtBeginning,

        /// <summary>
        /// The SPI transaction takes place, and once finished, CS is pulsed high
        /// </summary>
        PulseHighAtEnd,

        /// <summary>
        /// CS is pulsed low, and then the SPI transaction takes place.
        /// </summary>
        PulseLowAtBeginning,

        /// <summary>
        /// The SPI transaction takes place, and once finished, CS is pulsed low
        /// </summary>
        PulseLowAtEnd
    }
}
