package io.treehopper;
import io.treehopper.events.*;

/**
 * Created by jay on 12/6/2016.
 */

public interface DigitalInPin extends DigitalPinBase {
    void addDigitalInValueChangedEventHandler(DigitalInValueChangedEventHandler handler);
    void removeDigitalInValueChangedEventHandler(DigitalInValueChangedEventHandler handler);
    void makeDigitalIn();
}
