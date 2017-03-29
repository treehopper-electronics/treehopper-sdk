package io.treehopper.interfaces;

/**
 * Created by jay on 12/6/2016.
 */

public interface DigitalOutPin extends DigitalPinBase {
    void toggleOutput();
    void makeDigitalPushPullOutput();
}
