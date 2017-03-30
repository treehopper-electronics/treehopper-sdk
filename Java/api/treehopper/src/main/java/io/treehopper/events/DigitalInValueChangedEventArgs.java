package io.treehopper.events;

/**
 * Digital value changed EventArgs
 */
public class DigitalInValueChangedEventArgs {
    public boolean newDigitalValue;

    public DigitalInValueChangedEventArgs(boolean newDigitalValue) {
        this.newDigitalValue = newDigitalValue;
    }
}
