namespace Treehopper
{
    /// <summary>
    /// This interface provides generic access to any pin that supports hardware PWM operation.
    /// </summary>
    public interface IPwmPin
    {
        Pwm Pwm { get; set; }
    }
}
