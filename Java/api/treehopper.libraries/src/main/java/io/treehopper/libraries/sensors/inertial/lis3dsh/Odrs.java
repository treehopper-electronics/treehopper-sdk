/// This file was auto-generated by RegisterGenerator. Any changes to it will be overwritten!
package io.treehopper.libraries.sensors.inertial.lis3dsh;

 enum Odrs
{
    PowerDown (0),
    Hz_3P125 (1),
    Hz_6P25 (2),
    Hz_12P5 (3),
    Hz_25 (4),
    Hz_50 (5),
    Hz_100 (6),
    Hz_400 (7),
    Hz_800 (8),
    Hz_1600 (9);

int val;

Odrs(int val) { this.val = val; }
public int getVal() { return val; }
}
