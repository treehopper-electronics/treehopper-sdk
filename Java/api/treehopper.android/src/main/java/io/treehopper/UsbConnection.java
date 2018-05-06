package io.treehopper;

import android.content.Context;
import android.hardware.usb.*;

import io.treehopper.*;
import io.treehopper.interfaces.Connection;

/**
 * Android Connection implementation
 */
public class UsbConnection implements Connection {
    private UsbDeviceConnection connection;
    private UsbManager usbManager;
    UsbDevice usbDevice;

    UsbEndpoint pinConfigEndpoint;
    UsbEndpoint pinReportEndpoint;
    UsbEndpoint peripheralConfigEndpoint;
    UsbEndpoint peripheralResponseEndpoint;

    Thread pinListenerThread;
    boolean pinListenerThreadRunning = false;
    private TreehopperUsb board;

    public UsbConnection(UsbDevice device, Context appContext) {
        this(device, (UsbManager) appContext.getSystemService(appContext.USB_SERVICE));
    }

    public UsbConnection(UsbDevice device, UsbManager manager) {
        usbDevice = device;
        serialNumber = device.getSerialNumber();
        name = device.getProductName();
        usbManager = manager;
    }

    public boolean open() {
        if (connected)
            return true;


        UsbInterface intf = usbDevice.getInterface(0);
        pinReportEndpoint = intf.getEndpoint(0);
        peripheralResponseEndpoint = intf.getEndpoint(1);
        pinConfigEndpoint = intf.getEndpoint(2);
        peripheralConfigEndpoint = intf.getEndpoint(3);
        connection = usbManager.openDevice(usbDevice);
        if (connection != null) {
            boolean intfClaimed = connection.claimInterface(intf, true);
            if (intfClaimed) {
                connected = true;

                pinListenerThread = new Thread() {
                    @Override
                    public void run() {
                        byte[] data = new byte[64];
                        pinListenerThreadRunning = true;
                        while (pinListenerThreadRunning) {
                            int res = connection.bulkTransfer(pinReportEndpoint, data, 41, 100); // pin reports are 41 bytes long now
                            if (board != null && res > 0)
                                board.onPinReportReceived(data);
                        }
                    }
                };

                pinListenerThread.start();
            }
        }

        return connected;
    }

    boolean connected;

    public boolean isConnected() {
        return connected;
    }

    public void close() {
        if (!connected)
            return;

        pinListenerThreadRunning = false;
        try {
            pinListenerThread.join();
        } catch (InterruptedException e) {
            e.printStackTrace();
        }

        connection.close();
        connected = false;
    }

    public String getSerialNumber() {
        return serialNumber;
    }

    String serialNumber;

    public String getName() {
        return name;
    }

    String name;

    public void sendDataPinConfigChannel(byte[] data) {
        if (connection == null) {
            connected = false;
            return;
        }

        int res = connection.bulkTransfer(pinConfigEndpoint, data, data.length, 1000);
    }

    public void sendDataPeripheralChannel(byte[] data) {
        if (connection == null) {
            connected = false;
            return;
        }

        int res = connection.bulkTransfer(peripheralConfigEndpoint, data, data.length, 1000);
    }

    @Override
    public byte[] readPeripheralResponsePacket(int numBytesToRead) {
        byte[] data = new byte[numBytesToRead];
        int res = connection.bulkTransfer(peripheralResponseEndpoint, data, numBytesToRead, 1000);
        return data;
    }

    @Override
    public void setPinReportListener(TreehopperUsb board) {
        this.board = board;
    }

}
