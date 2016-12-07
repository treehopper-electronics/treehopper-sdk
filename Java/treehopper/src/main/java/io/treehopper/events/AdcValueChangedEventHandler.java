package io.treehopper.events;

/**
 * Created by jay on 12/6/2016.
 */

public interface AdcValueChangedEventHandler {
    void analogValueChanged(Object sender, AdcValueChangedEventArgs e);
}
