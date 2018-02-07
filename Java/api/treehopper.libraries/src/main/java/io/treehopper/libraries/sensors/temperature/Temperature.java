package io.treehopper.libraries.sensors.temperature;

import io.treehopper.libraries.sensors.Pollable;

/**
 * Temperature sensor interface
 */
public interface Temperature extends Pollable {
    double getCelsius();

    double getFahrenheit();

    double getKelvin();
}
