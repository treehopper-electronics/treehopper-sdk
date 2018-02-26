package io.treehopper.libraries.sensors.pressure;

import io.treehopper.libraries.sensors.IPollable;

public abstract class PressureSensor implements IPollable {

    protected boolean autoUpdateWhenPropertyRead;
    protected double pascal = 0;


    @Override
    public boolean isAutoUpdateWhenPropertyRead() {
        return autoUpdateWhenPropertyRead;
    }

    @Override
    public void setAutoUpdateWhenPropertyRead(boolean value) {
        autoUpdateWhenPropertyRead = value;
    }

    public double getPascal() {
        if (isAutoUpdateWhenPropertyRead())
            update();

        return pascal;
    }

    public double getBar() {
        return getPascal() / 100000.0;
    }

    public double getAtm() {
        return getPascal() / (1.01325 * 100000.0);
    }

    public double getPsi() {
        return getAtm() / 14.7;
    }
}
