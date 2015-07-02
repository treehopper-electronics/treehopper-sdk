namespace Treehopper
{
    /// <summary>
    /// This interface provides generic access to any pin that supports analog-to-digital conversion.
    /// </summary>
    public interface IAnalogInPin
    {
        AnalogIn AnalogIn { get; set; }
    }
}
