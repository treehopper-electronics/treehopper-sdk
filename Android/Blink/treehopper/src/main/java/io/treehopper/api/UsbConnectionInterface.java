package io.treehopper.api;

/**
 * Created by jay on 12/28/2015.
 */
public interface UsbConnectionInterface {
    boolean open();
    boolean isConnected();
    void close();
    String getSerialNumber();
    String getName();
    void sendDataPinConfigChannel(byte[] data);
    void sendDataPeripheralChannel(byte[] data);
    void setBoardReference(TreehopperUsb board);
}
