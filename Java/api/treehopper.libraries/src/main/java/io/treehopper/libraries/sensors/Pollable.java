package io.treehopper.libraries.sensors;

import io.treehopper.libraries.sensors.IPollable;

public abstract class Pollable implements IPollable {

    private boolean autoUpdateWhenPropertyRead = false;

    @Override
    public boolean isAutoUpdateWhenPropertyRead() {
        return autoUpdateWhenPropertyRead;
    }

    @Override
    public void setAutoUpdateWhenPropertyRead(boolean value) {
        autoUpdateWhenPropertyRead = value;
    }
}
