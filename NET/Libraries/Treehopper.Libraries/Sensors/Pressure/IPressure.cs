namespace Treehopper.Libraries.Sensors.Pressure
{
    interface IPressure : IPollable
    {
        double Pascal { get; }
    }
}
