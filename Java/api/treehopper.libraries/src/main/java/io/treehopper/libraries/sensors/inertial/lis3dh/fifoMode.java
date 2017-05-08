package io.treehopper.libraries.sensors.inertial.lis3dh;

 enum fifoMode
{
    Bypass (0),
    Fifo (1),
    Stream (2),
    StreamToFifo (3);

int val;

fifoMode(int val) { this.val = val; }
public int getVal() { return val; }
}
