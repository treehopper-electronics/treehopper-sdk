package io.treehopper;

/**
 * Created by jay on 12/4/2016.
 */

public interface I2c {
    byte[] sendReceive(byte address, byte[] dataToWrite, byte numBytesToRead);

    boolean isEnabled();
    void setEnabled(boolean enabled);

    double getSpeed();
    void setSpeed(double speed);

}
