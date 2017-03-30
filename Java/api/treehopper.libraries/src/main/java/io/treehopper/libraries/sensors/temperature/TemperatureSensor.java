package io.treehopper.libraries.sensors.temperature;

/**
 * Temperature sensor
 */
public abstract class TemperatureSensor implements Temperature {

    @Override
    public double getTemperatureFahrenheit() {
        return getTemperatureCelsius() * 9.0 / 5.0 + 32.0;
    }

    @Override
    public double getTemperatureKelvin() {
        return getTemperatureCelsius() + 273.15;
    }
}
