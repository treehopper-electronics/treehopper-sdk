package io.treehopper.libraries.io;

/**
 * An output interface that can be flushed
 */
public interface IFlushable {
    boolean isAutoFlushEnabled();
    void setAutoFlushEnabled(boolean value);
    void flush(boolean force);
}