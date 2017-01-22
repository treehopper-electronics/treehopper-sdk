namespace Treehopper
{
    /// <summary>
    /// Whether to use UART or OneWire mode
    /// </summary>
    public enum UartMode
    {
        /// <summary>
        /// The module is operating in UART mode
        /// </summary>
        Uart,

        /// <summary>
        /// The module is operating in OneWire mode. Only the TX pin is used.
        /// </summary>
        OneWire
    }
}
