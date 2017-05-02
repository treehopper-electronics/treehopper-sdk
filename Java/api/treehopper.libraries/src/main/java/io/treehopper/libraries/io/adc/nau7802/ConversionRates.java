package io.treehopper.libraries.io.adc.nau7802;

public enum ConversionRates
{
    Sps_10 (0),
    Sps_20 (1),
    Sps_40 (2),
    Sps_80 (3),
    Sps_320 (7);

int val;

ConversionRates(int val) { this.val = val; }
public int getVal() { return val; }
}
