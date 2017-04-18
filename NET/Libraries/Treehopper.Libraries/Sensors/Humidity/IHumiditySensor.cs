namespace Treehopper.Libraries.Sensors.Humidity
{
    internal interface IHumiditySensor : IPollable
    {
        double RelativeHumidity { get; }
    }
}