/// This file was auto-generated by RegisterGenerator. Any changes to it will be overwritten!
package io.treehopper.libraries.sensors.inertial.bno055;

 enum PowerModes
{
    Normal (0),
    LowPower (1),
    Suspend (2);

int val;

PowerModes(int val) { this.val = val; }
public int getVal() { return val; }
}