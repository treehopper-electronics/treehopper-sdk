package io.treehopper.libraries.sensors;

public interface Pollable {
    boolean isAutoUpdateWhenPropertyRead();
    void setAutoUpdateWhenPropertyRead(boolean value);
    void update();
}
