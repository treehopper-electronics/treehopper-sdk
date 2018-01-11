package io.treehopper.libraries.sensors.optical.vcnl4010;

 enum Rates
{
    Hz_1_95 (0),
    Hz_3_90625 (1),
    Hz_7_8125 (2),
    Hz_16_625 (3),
    Hz_31_25 (4),
    Hz_62_5 (5),
    Hz_125 (6),
    Hz_250 (7);

int val;

Rates(int val) { this.val = val; }
public int getVal() { return val; }
}
