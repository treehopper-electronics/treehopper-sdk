package io.treehopper.libraries.sensors.optical.bh1750;

public enum Resolution {
    Medium(0),
    High(1),
    Low(2);

    int val;

    Resolution(int val) {
        this.val = val;
    }

    public int getVal() {
        return val;
    }
}
