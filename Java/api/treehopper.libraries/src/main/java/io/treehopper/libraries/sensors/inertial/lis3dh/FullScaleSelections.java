package io.treehopper.libraries.sensors.inertial.lis3dh;

 enum FullScaleSelections
{
    scale_2G (0),
    scale_4G (1),
    scale_8G (2),
    scale_16G (3);

int val;

FullScaleSelections(int val) { this.val = val; }
public int getVal() { return val; }
}
