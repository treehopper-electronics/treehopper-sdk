package io.treehopper.libraries.sensors.inertial;

import com.badlogic.gdx.math.Vector3;

public abstract class Accelerometer implements IAccelerometer {
    protected boolean autoUpdateWhenPropertyRead;
    protected Vector3 accelerometer;

    @Override
    public Vector3 getAccelerometer() {
        if (autoUpdateWhenPropertyRead)
            update();

        return accelerometer;
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
