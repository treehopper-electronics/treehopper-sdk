package io.treehopper.libraries.io.adc.ads1115;

public enum Pgas
{
    Fsr_6144 (0),
    Fsr_4096 (1),
    Fsr_2048 (2),
    Fsr_1024 (3),
    Fsr_512 (4),
    Fsr_256 (5);

int val;

Pgas(int val) { this.val = val; }
public int getVal() { return val; }
}
