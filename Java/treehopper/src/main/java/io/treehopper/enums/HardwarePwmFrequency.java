package io.treehopper.enums;

/**
 * Created by jay on 12/6/2016.
 */

/// <summary>
/// Defines the PWM period options
/// </summary>
public enum HardwarePwmFrequency {
    /// <summary>
    /// 732 Hz PWM frequency
    /// </summary>
    Freq_732Hz(732),

    /// <summary>
    /// 183 Hz PWM frequency
    /// </summary>
    Freq_183Hz(183),

    /// <summary>
    /// 61 Hz PWM frequency
    /// </summary>
    Freq_61Hz(61);

    HardwarePwmFrequency(int frequency) {
        this.frequency = frequency;
    }

    int frequency;

    public int getFrequencyHz() {
        return frequency;
    }

}
