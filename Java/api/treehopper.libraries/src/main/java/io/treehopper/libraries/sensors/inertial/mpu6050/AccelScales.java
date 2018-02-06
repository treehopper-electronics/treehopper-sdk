package io.treehopper.libraries.sensors.inertial.mpu6050;

public enum AccelScales
{
    Fs_2g (0),
    Fs_4g (1),
    Fs_8g (2),
    Fs_16g (3);

int val;

AccelScales(int val) { this.val = val; }
public int getVal() { return val; }
}
