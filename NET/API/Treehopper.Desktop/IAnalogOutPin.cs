namespace Treehopper
{
    /// <summary>
    /// This interface provides generic access to any pin that supports analog voltage outputs.
    /// </summary>
    public interface IAnalogOutPin
    {
        AnalogOut AnalogOut { get; set; }
    }
}
