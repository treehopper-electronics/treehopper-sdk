package io.treehopper.libraries.sensors.optical.vcnl4010;

 enum IntCountExceeds
{
    count_1 (0),
    count_2 (1),
    count_4 (2),
    count_8 (3),
    count_16 (4),
    count_32 (5),
    count_64 (6),
    count_128 (7);

int val;

IntCountExceeds(int val) { this.val = val; }
public int getVal() { return val; }
}