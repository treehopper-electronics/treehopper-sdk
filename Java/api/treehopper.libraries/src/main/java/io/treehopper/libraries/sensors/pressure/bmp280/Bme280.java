package io.treehopper.libraries.sensors.pressure.bmp280;

import io.treehopper.SMBusDevice;
import io.treehopper.interfaces.I2c;
import io.treehopper.libraries.sensors.humidity.IHumiditySensor;

import java.util.ArrayList;
import java.util.List;

public class Bme280 extends Bmp280 implements IHumiditySensor {
    private final short h4;
    private final short h5;
    private double relativeHumidity = 0;

    public Bme280(I2c i2c, boolean sdoPin, int rate) {
        super(i2c, sdoPin, rate);

        registers.readRange(registers.h2, registers.h6);

        // RegisterGenerator doesn't get the endianness right on the h4/h5 12-bit values, so manually create them:
        h4 = (short) ((short) ((registers.h4.value << 4 | registers.h4h5.h4Low) << 4) >> 4);
        h5 = (short) ((short) ((registers.h5.value << 4 | registers.h4h5.h5Low) << 4) >> 4);

        registers.ctrlHumidity.setOversampling(Oversamplings.Oversampling_x16);
        registers.ctrlHumidity.write();
        registers.ctrlMeasure.write();
    }

    public static List<Bme280> Probe(I2c i2c) {
        List<Bme280> devs = new ArrayList<>();

        try {
            SMBusDevice dev = new SMBusDevice((byte) 0x76, i2c, 100);
            byte whoAmI = dev.readByteData((byte) 0xD0);
            if (whoAmI == (byte) 0x60) {
                devs.add(new Bme280(i2c, false, 100));
            }
        } catch (Exception ex) {
        }

        try {
            SMBusDevice dev = new SMBusDevice((byte) 0x76, i2c, 100);
            byte whoAmI = dev.readByteData((byte) 0xD0);
            if (whoAmI == (byte) 0x60) {
                devs.add(new Bme280(i2c, true, 100));
            }
        } catch (Exception ex) {
        }

        return devs;
    }

    @Override
    public void update() {
        super.update();

        // now the BME stuff
        double var_H;

        var_H = this.tFine - 76800.0;
        var_H = (registers.humidity.value - (h4 * 64.0 + h5 / 16384.0 * var_H)) *
                registers.h2.value / 65536.0 *
                (1.0 + registers.h6.value / 67108864.0 * var_H *
                        (1.0 + registers.h3.value / 67108864.0 * var_H));

        var_H = var_H * (1.0 - registers.h1.value * var_H / 524288.0);

        if (var_H > 100.0)
            var_H = 100.0;
        else if (var_H < 0.0)
            var_H = 0.0;

        relativeHumidity = var_H;
    }

    @Override
    public double getRelativeHumidity() {
        if (isAutoUpdateWhenPropertyRead())
            update();

        return relativeHumidity;
    }
}
