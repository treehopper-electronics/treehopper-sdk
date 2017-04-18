using System.Numerics;

namespace Treehopper.Libraries.Sensors.Inertial
{
    /// <summary>
    ///     Three-axis magnetometer (digital compass)
    /// </summary>
    public interface IMagnetometer : IPollable
    {
        /// <summary>
        ///     Gets the three-axis magnetometer value
        /// </summary>
        Vector3 Magnetometer { get; }
    }
}