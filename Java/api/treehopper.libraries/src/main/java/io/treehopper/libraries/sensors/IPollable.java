package io.treehopper.libraries.sensors;

public interface IPollable {
    boolean isAutoUpdateWhenPropertyRead();

    void setAutoUpdateWhenPropertyRead(boolean value);

    void update();
}
