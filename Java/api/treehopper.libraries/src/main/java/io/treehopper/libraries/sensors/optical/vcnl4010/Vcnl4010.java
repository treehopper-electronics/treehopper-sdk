package io.treehopper.libraries.sensors.optical.vcnl4010;

import io.treehopper.SMBusDevice;
import io.treehopper.interfaces.I2c;
import io.treehopper.libraries.SMBusRegisterManagerAdapter;
import io.treehopper.libraries.sensors.ProximitySensor;
import io.treehopper.libraries.sensors.optical.IAmbientLightSensor;

import static java.lang.Math.pow;

public class Vcnl4010 extends ProximitySensor implements IAmbientLightSensor {
    double lux;
    double rawProximity;
    Vcnl4010Registers registers;

    public Vcnl4010(I2c i2c, int rate) {
        registers = new Vcnl4010Registers(new SMBusRegisterManagerAdapter(new SMBusDevice((byte) (0x13), i2c, rate)));
        registers.readRange(registers.command, registers.ambientLightParameters);
        registers.proximityRate.setRate(Rates.Hz_7_8125);
        registers.ledCurrent.irLedCurrentValue = 20;
        registers.ambientLightParameters.setAlsRate(AlsRates.Hz_10);
        registers.ambientLightParameters.autoOffsetCompensation = 1;
        registers.ambientLightParameters.averagingSamples = 5;
        registers.writeRange(registers.command, registers.ambientLightParameters);
    }

    @Override
    public double getLux() {
        if (isAutoUpdateWhenPropertyRead())
            update();

        return lux;
    }

    public double getRawProximity() {
        if (isAutoUpdateWhenPropertyRead())
            update();
        return rawProximity;
    }

    @Override
    public void update() {
        // start ambient and prox conversion
        registers.command.alsOnDemandStart = 1;
        registers.command.proxOnDemandStart = 1;
        registers.command.write();

        while (true) {
            registers.command.read();
            if (registers.command.proxDataReady == 1 && registers.command.alsDataReady == 1)
                break;
        }

        registers.ambientLightResult.read();
        registers.proximityResult.read();

        // from datasheet
        lux = registers.ambientLightResult.value * 0.25;

        rawProximity = registers.proximityResult.value;

        // derived empirically
        if (registers.proximityResult.value < 2298)
            meters = Double.POSITIVE_INFINITY;
        else
            meters = 81.0 * pow(registers.proximityResult.value - 2298, -0.475) / 100; // empirically derived
    }
}
