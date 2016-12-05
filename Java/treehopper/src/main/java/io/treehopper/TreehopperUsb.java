package io.treehopper;

/**
 * Created by jay on 12/27/2015.
 */

public class TreehopperUsb {

    public Pin[] Pins = new Pin[20];
    private Connection usbConnection;
    boolean led;
    boolean connected;

    public TreehopperUsb(Connection connection) {
        usbConnection = connection;
        usbConnection.setPinReportListener(this);
        for (int i = 0; i < 20; i++)
            Pins[i] = new Pin(this, i);

    }

    public boolean getConnected() {
        return connected;
    }

    public boolean connect() {
        if(connected) return true;
        connected = true;
        return usbConnection.open();
    }

    public void disconnect() {
        if(!connected) return;
        connected = false;
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

    public Connection getConnection() {
        return usbConnection;
    }


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
    ConfigureDevice,    // Sent upon device connect/disconnect
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