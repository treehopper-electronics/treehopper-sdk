namespace Treehopper
{
    /// <summary>
    ///     A pin that can be used as an SPI chip-select
    /// </summary>
    public interface SpiChipSelectPin : DigitalOut
    {
        /// <summary>
        ///     The pin number of the pin
        /// </summary>
        int PinNumber { get; }

        /// <summary>
        ///     The SPI module that can use this pin
        /// </summary>
        Spi SpiModule { get; }
    }
}