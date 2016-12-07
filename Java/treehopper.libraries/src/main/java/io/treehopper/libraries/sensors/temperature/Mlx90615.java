package io.treehopper.libraries.sensors.temperature;

import io.treehopper.SMBusDevice;
import io.treehopper.interfaces.I2c;

/**
 * Created by jay on 12/7/2016.
 */

public class Mlx90615 {

    SMBusDevice dev;
    public Temperature ambient;
    public Temperature object;

    public Mlx90615(I2c module) {
        this.dev = new SMBusDevice((byte)0x5B, module, 100);

        ambient = new TempRegister(this.dev, (byte)0x26);
        object = new TempRegister(this.dev, (byte)0x27);
    }

    public double getRawIrData() {
        return dev.ReadWordData((byte)0x25);
    }
}

class TempRegister extends TemperatureSensor
{
    private SMBusDevice dev;
    private byte register;

    public TempRegister(SMBusDevice dev, byte register) {
        this.dev = dev;
        this.register = register;
    }

    @Override
    public double getTemperatureCelsius() {
        int data = dev.ReadWordData(register);

        data &= 0x00007FFF; // chop off the error bit of the high byte
        return (data * 0.02 - 273.15);
    }
}