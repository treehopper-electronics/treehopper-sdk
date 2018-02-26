package io.treehopper.libraries.sensors.temperature;

import io.treehopper.libraries.sensors.IPollable;

/**
 * Temperature sensor interface
 */
public interface Temperature extends IPollable {
    double getCelsius();

    double getFahrenheit();

    double getKelvin();
}
