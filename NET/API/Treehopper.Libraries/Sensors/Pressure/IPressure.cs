namespace Treehopper.Libraries.Sensors.Pressure
{
    internal interface IPressure : IPollable
    {
        double Pascal { get; }
    }
}