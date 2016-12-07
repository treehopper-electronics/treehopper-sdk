package io.treehopper.events;

/**
 * Created by jay on 12/6/2016.
 */

public class DigitalInValueChangedEventArgs {
    public DigitalInValueChangedEventArgs(boolean newDigitalValue) {
        this.newDigitalValue = newDigitalValue;
    }

    public boolean newDigitalValue;
}
