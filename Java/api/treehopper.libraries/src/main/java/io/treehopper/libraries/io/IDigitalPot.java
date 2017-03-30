package io.treehopper.libraries.io;

/**
 * Digital pot interface
 */
public interface IDigitalPot {
	public void setWiper(int wiper);
	public int getWiper();
	public void increment();
	public void decrement();
}
