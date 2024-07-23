package io.treehopper.libraries.sensors.optical.isl29125;

public interface InterruptReceivedEventHandler {
    void interruptReceived(Object sender, double red, double green, double blue);
}
