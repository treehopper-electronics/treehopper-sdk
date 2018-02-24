package io.treehopper.libraries.sensors.inertial.bno055;

 enum Modes
{
    normal (0),
    lowPower (1),
    suspend (2);

int val;

Modes(int val) { this.val = val; }
public int getVal() { return val; }
}
