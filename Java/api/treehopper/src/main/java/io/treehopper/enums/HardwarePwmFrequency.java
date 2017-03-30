package io.treehopper.enums;

/**
 * Hardware PWM frequency
 */

public enum HardwarePwmFrequency {

    /**
     * 732 Hz PWM frequency
     */
    Freq_732Hz(732),

    /**
     * 183 Hz PWM frequency
     */
    Freq_183Hz(183),

    /**
     * 61 Hz PWM frequency
     */
    Freq_61Hz(61);

    int frequency;

    HardwarePwmFrequency(int frequency) {
        this.frequency = frequency;
    }

    public int getFrequencyHz() {
        return frequency;
    }

}
