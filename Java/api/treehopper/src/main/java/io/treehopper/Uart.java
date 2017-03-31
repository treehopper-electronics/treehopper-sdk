package io.treehopper;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.util.ArrayList;

import io.treehopper.enums.UartMode;
import io.treehopper.interfaces.IOneWire;

/**
 * Hardware UART (with OneWire support)
 */
public class Uart implements IOneWire {
    private TreehopperUsb device;
    private UartMode mode;
    private boolean enabled;
    int baud;
    boolean openDrainTx;

    Uart(TreehopperUsb board) {
        this.device = board;
    }


    public UartMode getMode() {
        return mode;
    }


    public void setMode(UartMode mode) {
        this.mode = mode;
        updateConfig();
    }

    public boolean isEnabled() {
        return enabled;
    }

    public void setEnabled(boolean enabled) {
        this.enabled = enabled;
        updateConfig();
    }

    public int getBaud() {
        return baud;
    }

    public void setBaud(int baud) {
        this.baud = baud;
        updateConfig();
    }

    public boolean isOpenDrainTx() {
        return openDrainTx;
    }

    public void setOpenDrainTx(boolean openDrainTx) {
        this.openDrainTx = openDrainTx;
        updateConfig();
    }

    @Override
    public String toString() {
        if (enabled)
            return String.format("%s, running at %d baud", mode, baud);
        else
            return "Not enabled";
    }

    public void send(byte data) {
        send(new byte[] { data });
    }

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

    public byte[] receive(int numBytes)
    {
        byte[] retVal = new byte[0];
        if (mode == UartMode.Uart)
        {
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
            data[2] = (byte)numBytes;

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
     * Reset and match a device on the OneWire bus
     * @param address the address to reset and match
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