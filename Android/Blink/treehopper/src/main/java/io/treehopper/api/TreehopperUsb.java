package io.treehopper.api;

import java.util.HashMap;

import io.treehopper.api.android.ConnectionService;
import io.treehopper.api.android.UsbConnection;

/**
 * Created by jay on 12/27/2015.
 */
public class TreehopperUsb {
    public static ConnectionService getConnectionService() {
        return ConnectionService.getInstance();
    }

    private UsbConnectionInterface usbConnection;
    boolean led;

    public TreehopperUsb(UsbConnectionInterface connection) {
        usbConnection = connection;
        Pins.put(1, Pin1);
        Pins.put(2, Pin2);
        Pins.put(3, Pin3);
        Pins.put(4, Pin4);
        Pins.put(5, Pin5);
        Pins.put(6, Pin6);
        Pins.put(7, Pin7);
        Pins.put(8, Pin8);
        Pins.put(9, Pin9);
        Pins.put(10, Pin10);
        Pins.put(11, Pin11);
        Pins.put(12, Pin12);
        Pins.put(13, Pin13);
        Pins.put(14, Pin14);
        Pins.put(15, Pin15);
        Pins.put(16, Pin16);
        Pins.put(17, Pin17);
        Pins.put(18, Pin18);
        Pins.put(19, Pin19);
        Pins.put(20, Pin20);
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

    public HashMap<Integer, Pin> Pins = new HashMap<Integer, Pin>();

    // Pin definitions
    public Pin Pin1 = new Pin(this, 1, "P0.0 (SCK)" );
    public Pin Pin2 = new Pin(this, 2, "P0.1 (MISO)" );
    public Pin Pin3 = new Pin(this, 3, "P0.2 (MOSI)" );
    public Pin Pin4 = new Pin(this, 4, "P0.3 (SDA)" );
    public Pin Pin5 = new Pin(this, 5, "P0.6 (SCL)" );
    public Pin Pin6 = new Pin(this, 6, "P0.4 (TX)" );
    public Pin Pin7 = new Pin(this, 7, "P0.5 (RX)" );
    public Pin Pin8 = new Pin(this, 8, "P0.7 (PWM1)" );
    public Pin Pin9 = new Pin(this, 9, "P1.0 (PWM2)" );
    public Pin Pin10 = new Pin(this, 10, "P1.1 (PWM3)" );
    public Pin Pin11 = new Pin(this, 11, "P1.2" );
    public Pin Pin12 = new Pin(this, 12, "P1.3" );
    public Pin Pin13 = new Pin(this, 13, "P1.4" );
    public Pin Pin14 = new Pin(this, 14, "P1.5" );
    public Pin Pin15 = new Pin(this, 15, "P1.6" );
    public Pin Pin16 = new Pin(this, 16, "P1.7" );
    public Pin Pin17 = new Pin(this, 17, "P2.0" );
    public Pin Pin18 = new Pin(this, 18, "P2.1" );
    public Pin Pin19 = new Pin(this, 19, "P2.2" );
    public  Pin Pin20 = new Pin(this, 20, "P2.3" );
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