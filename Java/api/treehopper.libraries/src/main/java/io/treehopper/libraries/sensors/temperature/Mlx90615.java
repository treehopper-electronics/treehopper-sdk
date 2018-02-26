package io.treehopper.libraries.sensors.temperature;

import io.treehopper.SMBusDevice;
import io.treehopper.interfaces.I2c;

/**
 * Melexis MLX90615 non-contact I2c temperature sensor
 */
public class Mlx90615 extends Mlx90614 {

    public Temperature ambient;
    public Temperature object;
    SMBusDevice dev;

    public Mlx90615(I2c module) {
        super(module);

        ambient = new TempRegister(this.dev, (byte) 0x26);
        object = new TempRegister(this.dev, (byte) 0x27);
    }

    public double getRawIrData() {
        return dev.readWordData((byte) 0x25);
    }
}
