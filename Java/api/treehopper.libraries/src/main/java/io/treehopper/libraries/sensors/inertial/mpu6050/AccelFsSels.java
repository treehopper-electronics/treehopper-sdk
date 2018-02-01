package io.treehopper.libraries.sensors.inertial.mpu6050;

public enum AccelFsSels
{
    Fs_2g (0),
    Fs_4g (1),
    Fs_8g (2),
    Fs_16g (3);

int val;

AccelFsSels(int val) { this.val = val; }
public int getVal() { return val; }
}
