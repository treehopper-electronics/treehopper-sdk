package io.treehopper;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

/**
 * Created by jay on 12/27/2015.
 */

public class TreehopperUsb {

    public static Settings Settings = new Settings();

    private static final Logger logger = LogManager.getLogger("TreehopperUsb");

    public Pin[] pins = new Pin[20];
    private Connection usbConnection;
    private boolean led;
    private boolean connected;

    Object comsLock = new Object();

    public TreehopperUsb(Connection connection) {
        usbConnection = connection;
        usbConnection.setPinReportListener(this);
        for (int i = 0; i < 20; i++)
            pins[i] = new Pin(this, i);

    }

    public I2c i2c = new HardwareI2c(this);

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

    public boolean isLed() {
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
            for (Pin pin : pins) {
                pin.UpdateValue(pinReport[i++], pinReport[i++]);
            }
        }
    }

    public void sendPeripheralConfigPacket(byte[] dataToSend) {
        usbConnection.sendDataPeripheralChannel(dataToSend);
    }

    public byte[] receiveCommsResponsePacket(int numBytesToRead) {
        return usbConnection.readPeripheralResponsePacket(numBytesToRead);
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