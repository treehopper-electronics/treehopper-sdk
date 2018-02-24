package io.treehopper.libraries.sensors.temperature;

/**
 * Temperature sensor
 */
public abstract class TemperatureSensor implements Temperature {

    protected double celsius = 0;
    protected boolean autoUpdateWhenPropertyRead = true;

    public static double toKelvin(double celsius) {
        return celsius + 273.15;
    }

    public static double toFahrenheit(double celsius) {
        return celsius * 9.0 / 5.0 + 32.0;
    }

    @Override
    public double getCelsius() {
        if (autoUpdateWhenPropertyRead)
            update();

        return celsius;
    }

    @Override
    public boolean isAutoUpdateWhenPropertyRead() {
        return autoUpdateWhenPropertyRead;
    }

    @Override
    public void setAutoUpdateWhenPropertyRead(boolean value) {
        autoUpdateWhenPropertyRead = value;
    }

    @Override
    public double getFahrenheit() {
        return TemperatureSensor.toFahrenheit(getCelsius());
    }

    @Override
    public double getKelvin() {
        return TemperatureSensor.toKelvin(getCelsius());
    }
}
