namespace Treehopper
{
    /// <summary>
    /// This provides a generic interface to PWM operation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Since both the <see cref="SoftPwm"/> and <see cref="HardwarePwm"/> classes implement this interface, it is useful for applications 
    /// that are agnostic to differing resolutions or amount of jitter, and only need basic <see cref="DutyCycle"/> or <see cref="PulseWidth"/> control.
    /// </para>
    /// <para>
    /// Since <see cref="HardwarePwm"/> and <see cref="SoftPwm"/> have different periods (and different ways of setting the period), period control is not
    /// available in this interface. However, <see cref="HardwarePwm"/> initializes to a period of approximately 780 Hz and <see cref="SoftPwm"/> initializes with
    /// a period of approximately 100 Hz. These values are suitable for LED dimming, motor control, and other common applications. If higher (or lower) frequencies
    /// are desired, your library should use <see cref="HardwarePwm"/> or <see cref="SoftPwm"/> directly.
    /// </para>
    /// 
    /// </remarks>
    public interface Pwm
    {
        /// <summary>
        /// Get or set the duty cycle (0-1) of the pin
        /// </summary>
        double DutyCycle {get; set;}

        /// <summary>
        /// Get or set the pulse width, in microseconds, of the pin
        /// </summary>
        double PulseWidth { get; set; }

        /// <summary>
        /// Gets or sets the value determining whether the PWM functionality of the pin is enabled.
        /// </summary>
        bool Enabled { get; set; }
    }
}
