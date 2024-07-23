package io.treehopper.events;

/**
 * Digital value changed EventArgs
 */
public class DigitalInValueChangedEventArgs {
    public boolean newValue;

    public DigitalInValueChangedEventArgs(boolean newValue) {
        this.newValue = newValue;
    }
}
