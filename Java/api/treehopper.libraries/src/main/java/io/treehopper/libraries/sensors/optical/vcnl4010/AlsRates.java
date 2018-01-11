package io.treehopper.libraries.sensors.optical.vcnl4010;

 enum AlsRates
{
    Hz_1 (0),
    Hz_2 (1),
    Hz_3 (2),
    Hz_4 (3),
    Hz_5 (4),
    Hz_6 (5),
    Hz_8 (6),
    Hz_10 (7);

int val;

AlsRates(int val) { this.val = val; }
public int getVal() { return val; }
}
