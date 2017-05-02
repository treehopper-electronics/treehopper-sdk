package io.treehopper.libraries.io.adc.nau7802;

 enum AdcVcms
{
    ExtendedCommonModeRefp (3),
    ExtendedCommonModeRefn (2),
    disable (0);

int val;

AdcVcms(int val) { this.val = val; }
public int getVal() { return val; }
}
