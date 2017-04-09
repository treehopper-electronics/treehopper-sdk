package io.treehopper.libraries.sensors.temperature;
import io.treehopper.SMBusDevice;
import io.treehopper.interfaces.I2c;
/**
 * Created by jay on 2/12/2017.
 */

public class Mcp9808 extends TemperatureSensor {

    private final SMBusDevice device;

    public Mcp9808(I2c device)
    {
        this(device, (byte)0x18);
    }

    public Mcp9808(I2c device, boolean a0, boolean a1, boolean a2)
    {
        this(device, (byte)(0x18 | (a0 ? 1 : 0) | ((a1 ? 1 : 0) << 1) | ((a2 ? 1 : 0) << 2)));
    }

    public Mcp9808(I2c device, byte address)
    {
        this.device = new SMBusDevice(address, device);
    }

    @Override
    public double getTemperatureCelsius() {
        int data = device.ReadWordBE();
        double temp = data & 0x0FFF;
        temp /= 16.0;
        if ((data & 0x1000) > 0)
            temp -= 256;
        return temp;
    }
}