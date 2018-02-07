package io.treehopper.libraries.sensors.temperature;

import java.util.concurrent.TimeUnit;

import io.treehopper.interfaces.IOneWire;

/**
 * Maxim DS18B20 One-Wire temperature sensor
 */
public class Ds18b20 extends TemperatureSensor {
    private final IOneWire oneWire;
    private final long address;
    private boolean groupConversion;

    public Ds18b20(IOneWire oneWire, long address)
    {
        this.oneWire = oneWire;
        this.address = address;
        oneWire.startOneWire();
    }

    public Ds18b20(IOneWire oneWire)
    {
        this(oneWire, 0);
    }

    public boolean isGroupConversion() {
        return groupConversion;
    }

    public void setGroupConversion(boolean groupConversion) {
        this.groupConversion = groupConversion;
    }


    @Override
    public void update() {
        if(!groupConversion)
        {
            if(address == 0)
            {
                oneWire.oneWireReset();
                oneWire.send(new byte[] { (byte)0xCC, 0x44 });
            }
            else
            {
                oneWire.oneWireResetAndMatchAddress(address);
                oneWire.send((byte)0x44);
            }

            try {
                TimeUnit.MILLISECONDS.sleep(750);
            } catch (InterruptedException e) {

            }
        }

        if (address == 0)
        {
            oneWire.oneWireReset();
            oneWire.send(new byte[] { (byte)0xCC, (byte)0xBE });
        }
        else
        {
            oneWire.oneWireResetAndMatchAddress(address);
            oneWire.send((byte)0xBE);
        }

        byte[] data = oneWire.receive(2);

        celsius = ((short)(data[0] | (data[1] << 8))) / 16d;
    }
}
