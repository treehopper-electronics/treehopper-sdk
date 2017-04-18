namespace Treehopper.Libraries.Sensors.Humidity
{
    interface IHumiditySensor : IPollable
    {
        double RelativeHumidity { get; }
    }
}
