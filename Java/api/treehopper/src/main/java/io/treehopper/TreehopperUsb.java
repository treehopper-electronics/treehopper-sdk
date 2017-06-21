package io.treehopper;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import io.treehopper.interfaces.Connection;
import io.treehopper.interfaces.I2c;
import io.treehopper.interfaces.Pwm;
import io.treehopper.interfaces.Spi;

/**
 * Main Treehopper USB board class
 */
public class TreehopperUsb {

    private static final Logger logger = LogManager.getLogger("TreehopperUsb");
    public static Settings Settings = new Settings();

    public Pin[] pins = new Pin[20];
    public I2c i2c;
    public Spi spi;
    public Pwm pwm1;
    public Pwm pwm2;
    public Pwm pwm3;
    public HardwarePwmManager hardwarePwmManager;
    public Uart uart;
    Object comsLock = new Object();
    private Connection usbConnection;
    private boolean led;
    private boolean connected;

    SoftPwmManager softPwmManager;

    /**
     * Construct a Treehopper from a Connection
     *
     * @param connection the connection to use
     */
    public TreehopperUsb(Connection connection) {
        usbConnection = connection;
        usbConnection.setPinReportListener(this);
        for (int i = 0; i < 20; i++)
            pins[i] = new Pin(this, i);

        pwm1 = new HardwarePwm(pins[7]);
        pwm2 = new HardwarePwm(pins[8]);
        pwm3 = new HardwarePwm(pins[9]);

        i2c = new HardwareI2c(this);
        spi = new HardwareSpi(this);
        uart = new Uart(this);
        softPwmManager = new SoftPwmManager(this);

        hardwarePwmManager = new HardwarePwmManager(this);
    }


    /**
     * Get whether the board is connected
     *
     * @return the connection state of the board
     */
    public boolean getConnected() {
        return connected;
    }

    /**
     * Connect to the board and reinitialize all peripherals
     *
     * @return
     */
    public boolean connect() {
        if (connected) return true;
        connected = true;
        if (!usbConnection.open()) {
            return false;
        }

        reinitialize();
        return true;
    }

    /**
     * Disconnect from the board
     */
    public void disconnect() {
        if (!connected) return;

        reinitialize(); // reset all pins to inputs

        connected = false;
        usbConnection.close();
    }

    /**
     * Reinitialize the board, setting all pins as digital inputs
     */
    public void reinitialize() {
        byte[] data = new byte[2];
        data[0] = (byte) DeviceCommands.ConfigureDevice.ordinal();
        sendPeripheralConfigPacket(data);
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

    /**
     * Get the connection associated with this Treehopper board
     *
     * @return the Connection associated with this board
     */
    public Connection getConnection() {
        return usbConnection;
    }


    /**
     * Internal event used by the Connection to report pin changes
     *
     * @param pinReport the pin report received
     */
    public void onPinReportReceived(byte[] pinReport) {
        if (pinReport[0] != 0x00) {
            int i = 1;
            for (Pin pin : pins) {
                pin.UpdateValue(pinReport[i++], pinReport[i++]);
            }
        }
    }

    /**
     * send a peripheral configuration packet directly to the connection
     *
     * @param dataToSend The data to send
     */
    public void sendPeripheralConfigPacket(byte[] dataToSend) {
        usbConnection.sendDataPeripheralChannel(dataToSend);
    }

    /**
     * receive a peripheral response packet from the connection
     *
     * @param numBytesToRead the number of bytes to read
     * @return the data
     */
    public byte[] receiveCommsResponsePacket(int numBytesToRead) {
        return usbConnection.readPeripheralResponsePacket(numBytesToRead);
    }
}