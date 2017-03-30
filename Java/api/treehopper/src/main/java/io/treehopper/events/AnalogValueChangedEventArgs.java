package io.treehopper.events;

/**
 * Analog value changed EventArgs
 */
public class AnalogValueChangedEventArgs {
    public double newAnalogValue;
    public double oldAnalogValue;

    public AnalogValueChangedEventArgs(double newAnalogValue, double oldAnalogValue) {
        this.newAnalogValue = newAnalogValue;
        this.oldAnalogValue = oldAnalogValue;
    }
}
