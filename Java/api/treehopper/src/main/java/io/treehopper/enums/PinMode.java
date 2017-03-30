package io.treehopper.enums;

/**
 * Pin mode
 */
public enum PinMode {

    /**
     * Pin is reserved for other use
     */
    Reserved,

    /**
     * Pin is a digital input
     */
    DigitalInput,

    /**
     * Pin is a push-pull output
     */
    PushPullOutput,

    /**
     * Pin is an open-drain output
     */
    OpenDrainOutput,

    /**
     * Pin is an analog input
     */
    AnalogInput,

    /**
     * Pin is unassigned
     */
    Unassigned
}
