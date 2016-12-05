package io.treehopper.libraries.io;

public interface IFlushable {
    boolean isAutoFlushEnabled();
    void setAutoFlushEnabled(boolean value);
    void flush(boolean force);
}