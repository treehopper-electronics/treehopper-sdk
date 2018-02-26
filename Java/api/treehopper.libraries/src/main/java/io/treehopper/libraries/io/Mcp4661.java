package io.treehopper.libraries.io;

import io.treehopper.SMBusDevice;
import io.treehopper.interfaces.I2c;

/**
 * Microchip MCP4661 8-bit digital pot
 */
public class Mcp4661 implements IDigitalPot {
    private SMBusDevice device;
    private boolean useNonVolatile;
    private int wiperSelect;

    public Mcp4661(I2c device) {
        this(device, (byte) 0x28);
        this.useNonVolatile = false;
        this.wiperSelect = 0;
    }

    public Mcp4661(I2c device, boolean a0, boolean a1, boolean a2) {
        this(device, (byte) (0x28 | (a0 ? 1 : 0) | ((a1 ? 1 : 0) << 1) | ((a2 ? 1 : 0) << 2)));
        this.useNonVolatile = false;
        this.wiperSelect = 0;
    }

    public Mcp4661(I2c device, byte address) {
        this.device = new SMBusDevice(address, device);
        this.useNonVolatile = false;
        this.wiperSelect = 0;
    }

    @Override
    public int getWiper() {
        if (getWiperSelect() == 0) {
            return device.readWordDataBE((byte) 0x0c);
        }
        return device.readWordDataBE((byte) 0x1c);

    }

    @Override
    public void setWiper(int wiper) {
        byte byteWiper = (byte) wiper;

        if (getWiperSelect() == 0) {
            device.writeByteData((byte) 0x00, byteWiper);
        } else {
            device.writeByteData((byte) 0x10, byteWiper);
        }

    }

    @Override
    public void increment() {
        if (getWiperSelect() == 0) {
            device.writeByte((byte) 0x14);
        } else {
            device.writeByte((byte) 0x14);
        }

    }

    @Override
    public void decrement() {
        if (getWiperSelect() == 0) {
            device.writeByte((byte) 0x18);
        } else {
            device.writeByte((byte) 0x18);
        }

    }

    public boolean isUseNonVolatile() {
        return useNonVolatile;
    }

    public void setUseNonVolatile(boolean useNonVolatile) {
        this.useNonVolatile = useNonVolatile;
    }

    public int getWiperSelect() {
        return wiperSelect;
    }

    public void setWiperSelect(int wiperSelect) {
        this.wiperSelect = wiperSelect;
    }

}
