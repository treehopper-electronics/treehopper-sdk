package io.treehopper.libraries.io.adc.ads1115;

public enum DataRates
{
    Sps_8 (0),
    Sps_16 (1),
    Sps_32 (2),
    Sps_64 (3),
    Sps_128 (4),
    Sps_250 (5),
    Sps_475 (6),
    Sps_860 (7);

int val;

DataRates(int val) { this.val = val; }
public int getVal() { return val; }
}
