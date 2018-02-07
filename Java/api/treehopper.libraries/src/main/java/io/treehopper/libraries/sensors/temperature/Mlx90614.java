package io.treehopper.libraries.sensors.temperature;

import io.treehopper.SMBusDevice;
import io.treehopper.interfaces.I2c;

/**
 * Melexis MLX90614 non-contact I2c temperature sensor
 */
public class Mlx90614 {

    public class TempRegister extends TemperatureSensor
    {
        private SMBusDevice dev;
        private byte register;

        public TempRegister(SMBusDevice dev, byte register) {
            this.dev = dev;
            this.register = register;
        }

        @Override
        public void update() {
            int data = dev.readWordData(register);
            data &= 0x7FFF; // chop off the error bit of the high byte
            this.celsius = data * 0.02 - 273.15;
        }
    }

    SMBusDevice dev;
    private Temperature ambient;
    private Temperature object;

    public Mlx90614(I2c module) {
        this.dev = new SMBusDevice((byte)0x5A, module, 30);

        ambient = new TempRegister(this.dev, (byte)0x06);
        object = new TempRegister(this.dev, (byte)0x07);
    }

	public Temperature getAmbient() {
		return ambient;
	}

	public Temperature getObject() {
		return object;
	}

}


