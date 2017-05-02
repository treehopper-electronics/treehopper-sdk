package io.treehopper.libraries.io.adc.nau7802;

public enum LdoVoltage
{
    mV_4500 (0),
    mV_4200 (1),
    mV_3900 (2),
    mV_3600 (3),
    mV_3300 (4),
    mV_3000 (5),
    mV_2700 (6),
    mV_2400 (7);

int val;

LdoVoltage(int val) { this.val = val; }
public int getVal() { return val; }
}
