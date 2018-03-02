using System.Numerics;

namespace Treehopper.Libraries.Sensors.Inertial
{
    /// <summary>
    ///     Three-axis gyroscope sensor
    /// </summary>
    public interface IGyroscope : IPollable
    {
        /// <summary>
        ///     The three-axis gyroscope data, in DPS (degrees per second)
        /// </summary>
        Vector3 Gyroscope { get; }
    }
}