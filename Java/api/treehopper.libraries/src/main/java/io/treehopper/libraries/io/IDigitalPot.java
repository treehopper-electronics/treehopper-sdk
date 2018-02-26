package io.treehopper.libraries.io;

/**
 * Digital pot interface
 */
public interface IDigitalPot {
    public int getWiper();

    public void setWiper(int wiper);

    public void increment();

    public void decrement();
}
