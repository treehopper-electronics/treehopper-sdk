package io.treehopper.libraries.io.adc.ads1115;

public enum ComparatorQueues
{
    AssertAfterOneConversion (0),
    AssertAfterTwoConversions (1),
    AssertAfterFourConversions (2),
    DisableComparator (3);

int val;

ComparatorQueues(int val) { this.val = val; }
public int getVal() { return val; }
}
