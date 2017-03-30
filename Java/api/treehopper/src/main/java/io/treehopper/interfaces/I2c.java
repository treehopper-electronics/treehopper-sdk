package io.treehopper.interfaces;

/**
 * I2c interface
 */
public interface I2c {
    byte[] sendReceive(byte address, byte[] dataToWrite, int numBytesToRead);

    boolean isEnabled();

    void setEnabled(boolean enabled);

    double getSpeed();

    void setSpeed(double speed);

}
