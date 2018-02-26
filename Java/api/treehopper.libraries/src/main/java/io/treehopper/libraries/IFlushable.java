package io.treehopper.libraries;

/**
 * An output interface that can be flushed
 */
public interface IFlushable {
    boolean isAutoFlushEnabled();

    void setAutoFlushEnabled(boolean value);

    void flush(boolean force);
}