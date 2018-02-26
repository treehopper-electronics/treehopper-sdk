package io.treehopper.libraries.sensors.optical.bh1750;

import io.treehopper.SMBusDevice;
import io.treehopper.interfaces.I2c;
import io.treehopper.libraries.sensors.optical.AmbientLight;

public class Bh1750 extends AmbientLight {

    private SMBusDevice dev;
    private Resolution resolution;

    public Bh1750(I2c i2c) {
        this(i2c, false, 100);
    }

    public Bh1750(I2c i2c, boolean addressPin, int rate) {
        dev = new SMBusDevice((byte) (addressPin ? 0x5c : 0x23), i2c, rate);
        dev.writeByte((byte) 0x07);
        setResolution(Resolution.High);
    }

    public Resolution getResolution() {
        return resolution;
    }

    public void setResolution(Resolution resolution) {
        if (this.resolution == resolution)
            return;

        this.resolution = resolution;
        dev.writeByte((byte) (0x10 | resolution.getVal()));
    }

    @Override
    public void update() {
        lux = dev.readWordBE() / 1.2;
    }
}
