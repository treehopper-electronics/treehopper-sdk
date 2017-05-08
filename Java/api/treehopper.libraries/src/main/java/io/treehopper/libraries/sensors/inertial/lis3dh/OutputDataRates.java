package io.treehopper.libraries.sensors.inertial.lis3dh;

 enum OutputDataRates
{
    PowerDown (0),
    Hz_1 (1),
    Hz_10 (2),
    Hz_25 (3),
    Hz_50 (4),
    Hz_100 (5),
    Hz_200 (6),
    Hz_400 (7),
    Hz_1600 (8),
    Hz_1344_5376 (9);

int val;

OutputDataRates(int val) { this.val = val; }
public int getVal() { return val; }
}
