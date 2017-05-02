package io.treehopper.libraries.io.adc.nau7802;

public enum Gains
{
    x1 (0),
    x4 (1),
    x2 (2),
    x8 (3),
    x16 (4),
    x32 (5),
    x64 (6),
    x128 (7);

int val;

Gains(int val) { this.val = val; }
public int getVal() { return val; }
}
