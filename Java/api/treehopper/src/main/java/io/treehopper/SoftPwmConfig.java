package io.treehopper;

/**
 * Internal SoftPWM config structure
 */
public class SoftPwmConfig {
    public SoftPwmConfig(Pin pin, int pulseWidthUs, boolean usePulseWidth)
    {
        this.Pin = pin;
        this.PulseWidthUs = pulseWidthUs;
        this.UsePulseWidth = usePulseWidth;
    }

    public Pin Pin;

    ///     Duty Cycle, from 0 to 1.
    public double DutyCycle;

    ///     Pulse Width, in Milliseconds
    public double PulseWidthUs;

    public boolean UsePulseWidth;

    public int Ticks;
}
