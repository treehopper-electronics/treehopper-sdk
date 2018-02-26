package io.treehopper.libraries.sensors.magnetic;

import com.badlogic.gdx.math.Vector3;

public abstract class Magnetometer implements IMagnetometer {

    protected Vector3 _magnetometer = new Vector3();
    protected boolean autoUpdateWhenPropertyRead = true;

    @Override
    public Vector3 getMagnetometer() {
        if (isAutoUpdateWhenPropertyRead())
            update();

        return _magnetometer;
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
