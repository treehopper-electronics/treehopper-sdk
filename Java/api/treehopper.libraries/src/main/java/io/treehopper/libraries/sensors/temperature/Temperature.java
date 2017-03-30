package io.treehopper.libraries.sensors.temperature;

/**
 * Temperature sensor interface
 */
public interface Temperature {
    double getTemperatureCelsius();

    double getTemperatureFahrenheit();

    double getTemperatureKelvin();
}
