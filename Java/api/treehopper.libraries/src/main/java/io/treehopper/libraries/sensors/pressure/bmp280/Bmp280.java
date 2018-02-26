package io.treehopper.libraries.sensors.pressure.bmp280;

import io.treehopper.SMBusDevice;
import io.treehopper.interfaces.I2c;
import io.treehopper.libraries.sensors.pressure.PressureSensor;
import io.treehopper.libraries.sensors.temperature.Temperature;
import io.treehopper.libraries.sensors.temperature.TemperatureSensor;

import java.util.ArrayList;
import java.util.List;

public class Bmp280 extends PressureSensor implements Temperature {
    protected Bmp280Registers registers;
    protected double tFine;
    private double celsius;
    private double referencePressure = 101325;
    private double altitude;

    public Bmp280(I2c i2c, boolean sdoPin, int rate) {
        registers = new Bmp280Registers(new SMBusDevice((byte) (0x76 | (sdoPin ? 1 : 0)), i2c, rate));

        registers.ctrlMeasure.setMode(Modes.Normal);
        registers.ctrlMeasure.setOversamplingPressure(OversamplingPressures.Oversampling_x16);
        registers.ctrlMeasure.setOversamplingTemperature(OversamplingTemperatures.Oversampling_x16);
        registers.ctrlMeasure.write();

        registers.readRange(registers.t1, registers.h1);
    }

    public static List<Bmp280> Probe(I2c i2c, boolean includeBme280) {
        List<Bmp280> devs = new ArrayList<>();

        try {
            SMBusDevice dev = new SMBusDevice((byte) 0x76, i2c, 100);
            byte whoAmI = dev.readByteData((byte) 0xD0);
            if (whoAmI == (byte) 0x58 || (whoAmI == 0x60 && includeBme280)) {
                devs.add(new Bmp280(i2c, false, 100));
            }
        } catch (Exception ex) {
        }

        try {
            SMBusDevice dev = new SMBusDevice((byte) 0x76, i2c, 100);
            byte whoAmI = dev.readByteData((byte) 0xD0);
            if (whoAmI == (byte) 0x58 || (whoAmI == 0x60 && includeBme280)) {
                devs.add(new Bmp280(i2c, true, 100));
            }
        } catch (Exception ex) {
        }

        return devs;
    }

    @Override
    public void update() {
        registers.readRange(registers.pressure, registers.humidity); // even though this the BMP280, assume it's a BME280 so the bus is less chatty.

        // From Appendix A of the Bosch BMP280 datasheet
        double var1 = (registers.temperature.value / 16384.0 - registers.t1.value / 1024.0) * registers.t2.value;
        double var2 = ((registers.temperature.value / 131072.0 - registers.t1.value / 8192.0) * (registers.temperature.value / 131072.0 - registers.t1.value / 8192.0)) * registers.t3.value;
        this.tFine = (var1 + var2);
        this.celsius = (var1 + var2) / 5120.0;

        double p;
        var1 = this.tFine / 2.0 - 64000.0;
        var2 = var1 * var1 * registers.p6.value / 32768.0;
        var2 = var2 + var1 * registers.p5.value * 2.0;
        var2 = (var2 / 4.0) + registers.p4.value * 65536.0;
        var1 = (registers.p3.value * var1 * var1 / 524288.0 + registers.p2.value * var1) / 524288.0;
        var1 = (1.0 + var1 / 32768.0) * registers.p1.value;
        if (var1 == 0.0) {
            // avoid exception caused by division by zero
        } else {
            p = 1048576.0 - registers.pressure.value;
            p = (p - (var2 / 4096.0)) * 6250.0 / var1;
            var1 = registers.p9.value * p * p / 2147483648.0;
            var2 = p * registers.p8.value / 32768.0;
            p = p + (var1 + var2 + registers.p7.value) / 16.0;
            this.pascal = p;
            double kelvin = TemperatureSensor.toKelvin(celsius);
            altitude = AltitudeFromPressure(kelvin, pascal);
        }
    }

    private double AltitudeFromPressure(double temperature, double pressure) {
        double M = 0.0289644; // molar mass of earths' air
        double g = 9.80665; // gravity
        double R = 8.31432; // universal gas constant
        if (referencePressure / pressure < 101325 / 22632.1) {
            double d = -0.0065;
            double e = 0;
            double j = Math.pow(pressure / referencePressure, R * d / (g * M));
            return e + temperature * (1 / j - 1) / d;
        }
        if (referencePressure / pressure < 101325 / 5474.89) {
            double e = 11000;
            double b = temperature - 71.5;
            double f = R * b * Math.log(pressure / referencePressure) / (-g * M);
            double l = 101325;
            double c = 22632.1;
            double h = R * b * Math.log(l / c) / (-g * M) + e;
            return h + f;
        }
        return Double.NaN;
    }

    @Override
    public double getCelsius() {
        if (isAutoUpdateWhenPropertyRead())
            update();

        return celsius;
    }

    @Override
    public double getFahrenheit() {
        return TemperatureSensor.toFahrenheit(getCelsius());
    }

    @Override
    public double getKelvin() {
        return TemperatureSensor.toKelvin(getCelsius());
    }

    public double getReferencePressure() {
        return referencePressure;
    }

    public void setReferencePressure(double referencePressure) {
        this.referencePressure = referencePressure;
    }

    public double getAltitude() {
        if (isAutoUpdateWhenPropertyRead())
            update();

        return altitude;
    }
}
