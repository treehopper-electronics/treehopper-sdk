package io.treehopper.libraries.sensors.optical;

public abstract class AmbientLightSensor implements IAmbientLightSensor {
    protected double lux;
    private boolean autoUpdateWhenPropertyRead = true;

    @Override
    public double getLux() {
        if (isAutoUpdateWhenPropertyRead())
            update();

        return lux;
    }

    @Override
    public boolean isAutoUpdateWhenPropertyRead() {
        return autoUpdateWhenPropertyRead;
    }

    @Override
    public void setAutoUpdateWhenPropertyRead(boolean value) {
        autoUpdateWhenPropertyRead = value;
    }
}
