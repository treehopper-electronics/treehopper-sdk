package io.treehopper.libraries.sensors.inertial.mpu6050;

public enum ClockSels
{
    InternalOscillator (0),
    AutoSelect (1),
    Reset (7);

int val;

ClockSels(int val) { this.val = val; }
public int getVal() { return val; }
}
