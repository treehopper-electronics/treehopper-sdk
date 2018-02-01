package io.treehopper.libraries.sensors.inertial.mpu6050;

public enum ExtSyncSets
{
    Disabled (0),
    TempOutL (1),
    GyroXoutL (2),
    GyroYoutL (3),
    GyroZoutL (4),
    AccelXoutL (5),
    AccelYoutL (6),
    AccelZoutL (7);

int val;

ExtSyncSets(int val) { this.val = val; }
public int getVal() { return val; }
}
