package io.treehopper.libraries.sensors.temperature;

import io.treehopper.SMBusDevice;

public class TempRegister extends TemperatureSensor
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
        data &= 0x7FFF; // chop off the error bit of the high byte
        return (data * 0.02 - 273.15);
    }
}