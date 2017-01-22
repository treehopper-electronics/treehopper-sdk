namespace Treehopper
{
    /// <summary>
    /// Base interface representing pins capable of reading analog values
    /// </summary>
    public interface AdcPin
    {
        /// <summary>
        /// Make this pin an ADC pin
        /// </summary>
        void MakeAnalogIn();
    }
}
