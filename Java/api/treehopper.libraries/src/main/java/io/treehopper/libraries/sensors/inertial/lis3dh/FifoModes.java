package io.treehopper.libraries.sensors.inertial.lis3dh;

 enum FifoModes
{
    Bypass (0),
    Fifo (1),
    Stream (2),
    StreamToFifo (3);

int val;

FifoModes(int val) { this.val = val; }
public int getVal() { return val; }
}
