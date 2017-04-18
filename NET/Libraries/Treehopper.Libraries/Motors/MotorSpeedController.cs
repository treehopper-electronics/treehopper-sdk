namespace Treehopper.Libraries.Motors
{
    /// <summary>
    /// Any H-bridge device capable of motor speed control
    /// </summary>
    public interface MotorSpeedController
    {
        /// <summary>
        /// Controls whether the speed controller is enabled
        /// </summary>
        bool Enabled { get; set; }
        /// <summary>
        /// Set or retrieve the speed -- from -1.0 to 1.0 -- of the motor
        /// </summary>
        double Speed { get; set; }
    }
}
