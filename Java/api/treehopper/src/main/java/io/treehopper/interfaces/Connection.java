package io.treehopper.interfaces;

import io.treehopper.TreehopperUsb;

/**
 * A USB connection to a TreehopperUsb instance
 */
public interface Connection {
    boolean open();

    void close();

    String getSerialNumber();

    String getName();

    //    short getVersion();
//    short getDevicePath();
    void sendDataPinConfigChannel(byte[] data);

    void sendDataPeripheralChannel(byte[] data);

    byte[] readPeripheralResponsePacket(int numBytesToRead);

    void setPinReportListener(TreehopperUsb board);
}
