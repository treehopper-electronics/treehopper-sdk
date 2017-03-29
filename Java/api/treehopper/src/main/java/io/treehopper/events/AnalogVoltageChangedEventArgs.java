package io.treehopper.events;

/**
 * Created by jay on 12/6/2016.
 */
public class AnalogVoltageChangedEventArgs {
    public AnalogVoltageChangedEventArgs(double newAnalogVoltage, double oldAnalogVoltage) {
        this.newAnalogVoltage = newAnalogVoltage;
        this.oldAnalogVoltage = oldAnalogVoltage;
    }

    public double newAnalogVoltage;
    public double oldAnalogVoltage;
}
