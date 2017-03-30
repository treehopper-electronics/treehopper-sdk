package io.treehopper.interfaces;

import io.treehopper.events.DigitalInValueChangedEventHandler;

/**
 * Digital input pin
 */
public interface DigitalIn extends DigitalBase {
    void addDigitalInValueChangedEventHandler(DigitalInValueChangedEventHandler handler);

    void removeDigitalInValueChangedEventHandler(DigitalInValueChangedEventHandler handler);

    void makeDigitalIn();
}
