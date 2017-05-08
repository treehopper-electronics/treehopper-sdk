package io.treehopper.libraries.sensors.inertial.lis3dh;

 enum HighPassFilterModeSelections
{
    NormalMode (0),
    ReferenceSignal (1),
    Normal (2),
    AutoresetOnInterrupt (3);

int val;

HighPassFilterModeSelections(int val) { this.val = val; }
public int getVal() { return val; }
}
