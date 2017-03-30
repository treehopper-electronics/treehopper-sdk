package io.treehopper.events;

/**
 * Analog voltage changed EventArgs
 */
public class AnalogVoltageChangedEventArgs {
    public double newAnalogVoltage;
    public double oldAnalogVoltage;

    public AnalogVoltageChangedEventArgs(double newAnalogVoltage, double oldAnalogVoltage) {
        this.newAnalogVoltage = newAnalogVoltage;
        this.oldAnalogVoltage = oldAnalogVoltage;
    }
}
