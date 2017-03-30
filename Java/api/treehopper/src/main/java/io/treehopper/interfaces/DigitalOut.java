package io.treehopper.interfaces;

/**
 * Digital output
 */
public interface DigitalOut extends DigitalBase {
    void toggleOutput();

    void makeDigitalPushPullOutput();

    void setDigitalValue(boolean value);
}
