using System.Numerics;

namespace Treehopper.Libraries.Sensors.Inertial
{
    /// <summary>
    ///     Three-axis accelerometer sensor
    /// </summary>
    public interface IAccelerometer : IPollable
    {
        /// <summary>
        ///     The three-axis acceleration reading, in g.
        /// </summary>
        Vector3 Accelerometer { get; }
    }
}