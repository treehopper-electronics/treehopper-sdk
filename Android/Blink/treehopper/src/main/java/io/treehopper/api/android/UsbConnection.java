package io.treehopper.api.android;

import android.content.Context;
import android.hardware.usb.*;

import io.treehopper.api.*;

/**
 * Created by jay on 12/27/2015.
 */
public class UsbConnection implements UsbConnectionInterface {
    private UsbDeviceConnection connection;
    private UsbManager usbManager;
    UsbDevice usbDevice;

    UsbEndpoint pinConfigEndpoint;
    UsbEndpoint pinReportEndpoint;
    UsbEndpoint peripheralConfigEndpoint;
    UsbEndpoint peripheralResponseEndpoint;

    public UsbConnection(UsbDevice device, Context appContext) {
        this(device, (UsbManager)appContext.getSystemService(appContext.USB_SERVICE));
    }

    public UsbConnection(UsbDevice device, UsbManager manager)
    {
        usbDevice = device;
        serialNumber = device.getSerialNumber();
        name = device.getProductName();
        usbManager = manager;
    }

    TreehopperUsb board;

    public boolean open() {
        if(connected)
            return false;

        UsbInterface intf = usbDevice.getInterface(0);
        pinReportEndpoint = intf.getEndpoint(0);
        peripheralResponseEndpoint = intf.getEndpoint(1);
        pinConfigEndpoint = intf.getEndpoint(2);
        peripheralConfigEndpoint = intf.getEndpoint(3);
        connection = usbManager.openDevice(usbDevice);
        if(connection != null)
        {
            boolean intfClaimed = connection.claimInterface(intf, true);
            if(intfClaimed)
            {
                connected = true;
                return true;
            }
        }
        return false;
    }
    boolean connected;

    public boolean isConnected()
    {
        return connected;
    }

    public void close() {
        if(!connected)
            return;

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
        if(connection == null)
        {
            connected = false;
            return;
        }

        connection.bulkTransfer(pinConfigEndpoint, data, data.length, 1000);
    }

    public void sendDataPeripheralChannel(byte[] data) {
        if(connection == null)
        {
            connected = false;
            return;
        }

        connection.bulkTransfer (peripheralConfigEndpoint, data, data.length, 1000);
    }

    @Override
    public void setBoardReference(TreehopperUsb board) {
        this.board = board;
    }
}
