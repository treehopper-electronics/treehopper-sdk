package io.treehopper;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import io.treehopper.interfaces.Connection;
import io.treehopper.interfaces.I2c;
import io.treehopper.interfaces.Pwm;
import io.treehopper.interfaces.Spi;

/** The core class for communicating with %Treehopper USB boards.

 ![Treehopper pinout](images/treehopper-hardware.svg)
 # Core Hardware {#core-hardware}
 %Treehopper is a USB 2.0 Full Speed device with 20 \link Pin Pins\endlink — each of which can be used as an analog input, digital input, digital output, or soft-PWM output. Many of these pins also have dedicated peripheral functions for \link HardwareSpi SPI\endlink, \link HardwareI2c I2C\endlink, \link HardwareUart UART\endlink, and \link HardwarePwm PWM\endlink.

 You'll access all the pins, peripherals, and board functions through this class, which will automatically create all peripheral instances for you.

 ## Getting a board reference
 To obtain a reference to the board, use the \link ConnectionService.getFirstDevice() ConnectionService.getInstance().getFirstDevice()\endlink method:

 ```{.java}
 TreehopperUsb board = ConnectionService.getInstance().getFirstDevice();
 ```

 Or the \link ConnectionService.getBoards() ConnectionService.getInstance().getBoards()\endlink vector.

 \warning While you're free to create TreehopperUsb variables that reference boards, never call %TreehopperUsb's constructor yourself.

 ## Connect to the board
 Before you use the board, you must explicitly connect to it by calling the ConnectAsync() method

 ```{.java}
 TreehopperUsb board = ConnectionService.getInstance().getFirstDevice();
 board.connect();
 ```

 \note Once a board is connected, other applications won't be able to use it. 

 ## Next steps
 To learn about accessing different %Treehopper peripherals, visit the doc links to the relevant classes:
 - Pin
 - HardwareSpi
 - HardwareI2c
 - HardwareUart
 - HardwarePwm
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
    public HardwareUart uart;
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
        uart = new HardwareUart(this);
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