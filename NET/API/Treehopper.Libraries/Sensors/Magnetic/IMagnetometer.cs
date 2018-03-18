using System.Numerics;

namespace Treehopper.Libraries.Sensors.Magnetic
{
    /// <summary>
    ///     Three-axis magnetometer (digital compass)
    /// </summary>
    public interface IMagnetometer : IPollable
    {
        /// <summary>
        ///     Gets the three-axis magnetometer value, in uT (micro-Tesla)
        /// </summary>
        Vector3 Magnetometer { get; }
    }
}