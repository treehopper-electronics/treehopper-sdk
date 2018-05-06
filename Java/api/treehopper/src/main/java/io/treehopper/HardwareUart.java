package io.treehopper;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.util.ArrayList;

import io.treehopper.enums.UartMode;
import io.treehopper.interfaces.IOneWire;

/** Built-in UART peripheral

 The UART peripheral allows you to send and receive standard-format RS-232-style asynchronous serial communications.

 ## Pins
 When the UART is enabled, the following pins will be unavailable for other use:
 - <b>TX</b> <i>(Transmit)</i>: This pin carries data from %Treehopper to the device you've attached to the UART.
 - <b>RX</b> <i>(Receive)</i>: This pin carries data from the device to %Treehopper.

 Note that UART cross-over is a common problem when people are attaching devices together; always consult the documentation for the device you're attaching to %Treehopper to ensure that the TX signal from %Treehopper is flowing into the receive input (RX, DIN, etc) of the device, and vice-versa. Since you are unlikely to damage either device by incorrectly connecting the TX and RX pins, it is a common troubleshooting practice to simply swap TX and RX if the system doesn't appear to be functioning properly.

 ## One-Wire Mode
 %Treehopper's UART has built-in support for One-Wire mode with few external circuitry requirements. When you use the UART in One-Wire mode, the TX pin will switch to an open-drain mode. You must physically tie the RX and TX pins together --- this is the data pin for the One-Wire bus. Most One-Wire sensors and devices you use will require an external pull-up resistor on the bus.

 ## Implementation Details
 %Treehopper's UART is designed for average baud rates; the range of supported rates is 7813 baud to 2.4 Mbaud, though communication will be less reliable above 1-2 Mbaud.

 Transmitting data is straightforward: simply pass a byte array --- up to 63 characters long --- to the send() function once the UART is enabled.

 Receiving data is more challenging, since incoming data can appear on the RX pin at any moment when the UART is enabled. Since all actions on %Treehopper are initiated on the host, to get around UART's inherent asynchronicity, a 32-byte buffer holds any received data that comes in while the UART is enabled. Then, when the host wants to access this data, it can receive() it from the board to obtain the buffer.

 Whenever receive() is called, the entire buffer is sent to the host, and the buffer's pointer is reset to 0 (i.e., the buffer is reset). This can be useful for clearing out any gibberish and returning the UART to a known state before you expect to receive data --- for example, if you're addressing a device that you send commands to, and read responses back from, you may wish to call receive() before sending the command; that way, parsing the received data will be simpler.

 ## Other Considerations
 This ping-pong short-packet-oriented back-and-forth scenario is what %Treehopper's UART is built for, as it's what's most commonly needed when interfacing with embedded devices that use a UART.

 There is a tight window of possible baud rates where it is plausible to receive data continuously without interruption. For example, at 9600 baud, the receive() function only need to finish execution every 33 milliseconds, which can easily be accomplished in most operating systems. However, because data is not double-buffered on the board, under improbable circumstances, continuously-transmitted data may inadvertently be discarded.

 %Treehopper's UART is not designed to replace a high-quality CDC-class USB-to-serial converter, especially for high data-rate applications. In addition to streaming large volumes of data continuously, USB CDC-class UARTs should also offer lower latency for receiving data. %Treehopper also has no way of exposing its UART to the operating system as a COM port, so it's most certainly not a suitable replacement for a USB-to-serial converter in most applications.
 */
public class HardwareUart implements IOneWire {
    private TreehopperUsb device;
    private UartMode mode;
    private boolean enabled;
    int baud;
    boolean openDrainTx;

    HardwareUart(TreehopperUsb board) {
        this.device = board;
    }

    /**
     * Get the current mode the UART is operating in.
     * @return the UART's current mode.
     */
    public UartMode getMode() {
        return mode;
    }

    /**
     * Sets the mode to operate the UART in.
     * @param mode the mode to use.
     */
    public void setMode(UartMode mode) {
        this.mode = mode;
        updateConfig();
    }

    /**
     * Gets whether the UART is enabled.
     * @return whether the UART is enabled.
     */
    public boolean isEnabled() {
        return enabled;
    }

    /**
     * Sets whether the UART should be enabled.
     * @param enabled whether the UART should be enabled.
     */
    public void setEnabled(boolean enabled) {
        this.enabled = enabled;
        updateConfig();
    }

    /**
     * Get the baud of the UART.
     * @return the current baud, in bits per second.
     */
    public int getBaud() {
        return baud;
    }

    /**
     * Set the baud of the UART.
     * @param baud the desired baud, in bits per second.
     */
    public void setBaud(int baud) {
        this.baud = baud;
        updateConfig();
    }

    /**
     * Determine whether the UART is using open-drain Tx mode.
     * @return whether the UART is using open-drain Tx mode.
     */
    public boolean isOpenDrainTx() {
        return openDrainTx;
    }

    /**
     * Sets whether the UART should use open-drain Tx mode.
     * @param openDrainTx whether the UART should use open-drain Tx mode.
     */
    public void setOpenDrainTx(boolean openDrainTx) {
        this.openDrainTx = openDrainTx;
        updateConfig();
    }

    /**
     * Gets a string representation of the current state of the UART.
     * @return
     */
    @Override
    public String toString() {
        if (enabled)
            return String.format("%s, running at %d baud", mode, baud);
        else
            return "Not enabled";
    }

    /**
     * Send a byte of data out the UART in UART or One-Wire mode.
     * @param data The byte to send
     */
    public void send(byte data) {
        send(new byte[] { data });
    }

    /**
     * Send an array of data out the UART in UART or One-Wire mode.
     * @param dataToSend  byte array of the data to send
     */
    public void send(byte[] dataToSend)
    {
        if (dataToSend.length > 63)
        {
            Utilities.error("The maximum UART length for one transaction is 63 bytes");
        }

        byte[] data = new byte[dataToSend.length + 3];
        data[0] = (byte)DeviceCommands.UartTransaction.ordinal();
        data[1] = (byte)UartCommand.Transmit.ordinal();
        data[2] = (byte)dataToSend.length;

        System.arraycopy(dataToSend, 0, data, 3, dataToSend.length);

        synchronized (device.comsLock) {
            device.sendPeripheralConfigPacket(data);
            byte[] receivedData = device.receiveCommsResponsePacket(1);
        }
    }

    /**
     * Receive the UART's entire RX buffer
     * @return the bytes in the RX buffer
     *
     * As soon as the UART is enabled, any received byte will be added to a 32-byte buffer. Calling this Receive() function does two things:
     *    - sends the current contents of this buffer to this function.
     *    - reset the pointer in the buffer to the 0th element, effectively resetting it.
     * If the buffer fills before the Receive() function is called, the existing buffer will be reset --- discarding all data in the buffer.
     * Consequently, it's important to call the Receive() function frequently when expecting data.
     *
     * Owing to how it is implemented, you can clear the buffer at any point by calling Receive(). It's common to empty the buffer before
     * requesting data from the device attached to the UART; this way, you do not have to worry about existing gibberish data that
     * might have been inadvertently received.
     */
    public byte[] receive()
    {
        return this.receive(0);
    }

    /**
     * Receive bytes from the UART in One-Wire Mode
     * @param oneWireNumBytes the number of bytes to request from the One-Wire device
     * @return the bytes received
     * This function overload should only be called when the UART is in One-Wire mode. Treehopper will request oneWireNumBytes from the device,
     * and return this data as an array.
     */
    public byte[] receive(int oneWireNumBytes)
    {
        byte[] retVal = new byte[0];
        if (mode == UartMode.Uart)
        {
            if(oneWireNumBytes != 0)
                Utilities.error("oneWireNumBytes is only used when the UART is in One-Wire mode");
            byte[] data = new byte[2];
            data[0] = (byte)DeviceCommands.UartTransaction.ordinal();
            data[1] = (byte)UartCommand.Receive.ordinal();

            synchronized (device.comsLock) {
                device.sendPeripheralConfigPacket(data);
                byte[] receivedData = device.receiveCommsResponsePacket(33);
                int len = receivedData[32];
                retVal = new byte[len];
                System.arraycopy(receivedData, 0, retVal, 0, len);
            }
        }
        else
        {
            byte[] data = new byte[3];
            data[0] = (byte)DeviceCommands.UartTransaction.ordinal();
            data[1] = (byte)UartCommand.Receive.ordinal();
            data[2] = (byte)oneWireNumBytes;

            synchronized (device.comsLock) {
                device.sendPeripheralConfigPacket(data);
                byte[] receivedData = device.receiveCommsResponsePacket(33);
                int len = receivedData[32];
                retVal = new byte[len];
                System.arraycopy(receivedData, 0, retVal, 0, len);
            }
        }

        return retVal;
    }

    private void updateConfig() {
        if (!enabled)
        {
            device.sendPeripheralConfigPacket(new byte[] { (byte)DeviceCommands.UartConfig.ordinal(), (byte)UartConfig.Disabled.ordinal() });
        }
        else if (mode == UartMode.Uart)
        {
            byte timerVal = 0;
            boolean usePrescaler = false;

            // calculate baud with and without prescaler
            int timerValPrescaler = (int)Math.round(256.0 - (2000000.0 / baud));
            int timerValNoPrescaler = (int)Math.round(256.0 - (24000000.0 / baud));

            boolean prescalerOutOfBounds = timerValPrescaler > 255 || timerValPrescaler < 0;
            boolean noPrescalerOutOfBounds = timerValNoPrescaler > 255 || timerValNoPrescaler < 0;

            // calculate error
            double prescalerError = Math.abs(baud - (2000000 / (256 - timerValPrescaler)));
            double noPrescalerError = Math.abs(baud - (24000000 / (256 - timerValNoPrescaler)));

            if (prescalerOutOfBounds && noPrescalerOutOfBounds)
            {
                Utilities.error("The specified baud rate was out of bounds.");
            }
            else if (prescalerOutOfBounds)
            {
                usePrescaler = false;
                timerVal = (byte)timerValNoPrescaler;
            }
            else if (noPrescalerOutOfBounds)
            {
                usePrescaler = true;
                timerVal = (byte)timerValPrescaler;
            }
            else if (prescalerError > noPrescalerError)
            {
                usePrescaler = false;
                timerVal = (byte)timerValNoPrescaler;
            }
            else
            {
                usePrescaler = true;
                timerVal = (byte)timerValPrescaler;
            }

            byte[] data = new byte[5];
            data[0] = (byte)DeviceCommands.UartConfig.ordinal();
            data[1] = (byte)UartConfig.Standard.ordinal();
            data[2] = timerVal;
            data[3] = (byte)(usePrescaler ? 0x01 : 0x00);
            data[4] = (byte)(openDrainTx ? 0x01 : 0x00);
            device.sendPeripheralConfigPacket(data);
        }
        else
        {
            byte[] data = new byte[2];
            data[0] = (byte)DeviceCommands.UartConfig.ordinal();
            data[1] = (byte)UartConfig.OneWire.ordinal();
            device.sendPeripheralConfigPacket(data);
        }
    }

    /**
     * Send a One-Wire reset command.
     * @return True if at least one device was found.
     */
    public boolean oneWireReset() {
        if (mode != UartMode.OneWire)
            Utilities.error("The UART must be in OneWire mode to issue a oneWireReset command");

        if(!enabled)
            Utilities.error("The UART must be enabled to issue a oneWireReset command");

        boolean retVal = false;
        byte[] data = new byte[2];
        data[0] = (byte)DeviceCommands.UartTransaction.ordinal();
        data[1] = (byte)UartCommand.OneWireReset.ordinal();
        synchronized (device.comsLock) {
            device.sendPeripheralConfigPacket(data);
            byte[] receivedData = device.receiveCommsResponsePacket(1);
            retVal = receivedData[0] > 0 ? true : false;
        }

        return retVal;
    }

    /**
     * Search for devices on the One-Wire bus.
     * @return a list of device addresses found on the One-Wire bus.
     */
    public ArrayList<Long> oneWireSearch()
    {
        if (mode != UartMode.OneWire)
            Utilities.error("The UART must be in OneWire mode to issue a oneWireSearch command");

        if(!enabled)
            Utilities.error("The UART must be enabled to issue a oneWireSearch command");

        ArrayList<Long> retVal = new ArrayList<Long>();
        byte[] data = new byte[2];
        data[0] = (byte)DeviceCommands.UartTransaction.ordinal();
        data[1] = (byte)UartCommand.OneWireScan.ordinal();
        synchronized (device.comsLock) {
            device.sendPeripheralConfigPacket(data);
            byte[] receivedData = new byte[8];
            while (true)
            {
                receivedData = device.receiveCommsResponsePacket(9);
                if (receivedData[0] == (byte)0xff)
                    break;

                ByteBuffer buffer = ByteBuffer.wrap(receivedData);
                buffer.order(ByteOrder.BIG_ENDIAN);
                retVal.add(buffer.getLong());
            }
        }

        return retVal;
    }

    /**
     * Reset and match a device on the OneWire bus.
     * @param address the address to reset and match.
     */
    public void oneWireResetAndMatchAddress(long address)
    {
        if (mode != UartMode.OneWire)
            Utilities.error("The UART must be in OneWire mode to issue a oneWireResetAndMatchAddress command");

        if(!enabled)
            Utilities.error("The UART must be enabled to issue a oneWireResetAndMatchAddress command");

        oneWireReset();

        ByteBuffer buffer = ByteBuffer.allocate(9).order(ByteOrder.BIG_ENDIAN);
        buffer.put((byte)0x55); // MATCH ROM
        buffer.putLong(address);

        send(buffer.array());
    }

    /**
     * Enable the UART and put it in One-Wire mode.
     */
    public void startOneWire()
    {
        setMode(UartMode.OneWire);
        setEnabled(true);
    }

    enum UartCommand {
        Transmit,
        Receive,
        OneWireReset,
        OneWireScan
    }

    enum UartConfig {
        Disabled,
        Standard,
        OneWire,
    }
}