package io.treehopper.libraries.sensors.temperature;

import io.treehopper.SMBusDevice;
import io.treehopper.interfaces.I2c;

public class Mlx90614 {
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


