package io.treehopper.api;

import java.util.ArrayList;

import io.treehopper.api.android.ConnectionService;
import io.treehopper.api.android.UsbConnection;

/**
 * Created by jay on 12/27/2015.
 */
public class TreehopperUsb implements PinReportListener {
    public static ConnectionService getConnectionService() {
        return ConnectionService.getInstance();
    }

    private UsbConnectionInterface usbConnection;
    boolean led;

    public TreehopperUsb(UsbConnectionInterface connection) {
        usbConnection = connection;
        usbConnection.setPinReportListener(this);
        for (int i = 0; i < 20; i++)
            Pins[i] = new Pin(this, i);

    }

    public boolean isConnected() {
        return usbConnection.isConnected();
    }

    public boolean connect() {
        return usbConnection.open();
    }

    public void disconnect() {
        usbConnection.close();
    }

    public String getName() {
        return usbConnection.getName();
    }

    public String getSerialNumber() {
        return usbConnection.getSerialNumber();
    }

    public boolean getLed() {
        return led;
    }

    public void setLed(boolean led) {
        this.led = led;
        byte[] DataToSend = new byte[2];
        DataToSend[0] = (byte) DeviceCommands.LedConfig.ordinal();
        DataToSend[1] = (byte) (led ? 0x01 : 0x00); // Unicode 16-bit strings are 2 bytes per character
        usbConnection.sendDataPeripheralChannel(DataToSend);
    }

    public UsbConnectionInterface getConnection() {
        return usbConnection;
    }

    public Pin[] Pins = new Pin[20];

    @Override
    public void onPinReportReceived(byte[] pinReport) {
        if (DeviceResponse.values()[pinReport[0]] == DeviceResponse.CurrentReadings) {
            int i = 1;
            for (Pin pin : Pins) {
                pin.UpdateValue(pinReport[i++], pinReport[i++]);
            }
        }
    }
}

enum DeviceCommands {
    Reserved,    // Not implemented
    GetDeviceInfo,    // Not implemented
    PinConfig,    // Configures a GPIO pin as an input/output
    ComparatorConfig,    // Not implemented
    PwmConfig,    // Configures the hardware DAC
    UartConfig,    // Not implemented
    I2cConfig,    // Configures i2C master
    SpiConfig,    // Configures SPI master
    I2cTransaction,    // (Endpoint 2) Performs an i2C transaction
    SpiTransaction,    // (Endpoint 2) Performs an SPI transaction
    SoftPwmConfig,    //
    ServoControllerConfig,
    FirmwareUpdateSerial,    //
    FirmwareUpdateName,    //
    Reboot,    //
    EnterBootloader,    //
    LedConfig
}

enum DeviceResponse {
    Reserved,
    DeviceInfo,
    CurrentReadings,
    UARTDataReceived,
    I2CDataReceived,
    SPIDataReceived
}