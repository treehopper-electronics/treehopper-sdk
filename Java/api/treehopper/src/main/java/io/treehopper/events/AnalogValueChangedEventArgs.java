package io.treehopper.events;

/**
 * Created by jay on 12/6/2016.
 */

public class AnalogValueChangedEventArgs {
    public AnalogValueChangedEventArgs(double newAnalogValue, double oldAnalogValue) {
        this.newAnalogValue = newAnalogValue;
        this.oldAnalogValue = oldAnalogValue;
    }

    public double newAnalogValue;
    public double oldAnalogValue;
}
