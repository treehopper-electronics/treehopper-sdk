package io.treehopper.libraries.sensors.temperature;

import io.treehopper.SMBusDevice;
import io.treehopper.interfaces.I2c;

/**
 * LM75 I2c temperature sensor
 */
public class Lm75 extends TemperatureSensor {
    private final SMBusDevice device;
    private boolean autoUpdateWhenPropertyRead = true;

    public Lm75(I2c device)
    {
        this(device, false, false, false);
    }

    public Lm75(I2c device, boolean a0, boolean a1, boolean a2)
    {
        this(device, (byte)(0x48 | (a0 ? 1 : 0) | ((a1 ? 1 : 0) << 1) | ((a2 ? 1 : 0) << 2)));
    }

    public Lm75(I2c device, byte address)
    {
        this.device = new SMBusDevice(address, device);
    }

    @Override
    public void update() {
        short data = device.readWordDataBE((byte)0x00);
        celsius = (data / 32.0) / 8.0;
    }
}