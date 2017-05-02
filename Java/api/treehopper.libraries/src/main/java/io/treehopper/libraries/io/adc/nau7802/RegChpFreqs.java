package io.treehopper.libraries.io.adc.nau7802;

 enum RegChpFreqs
{
    off (3);

int val;

RegChpFreqs(int val) { this.val = val; }
public int getVal() { return val; }
}
