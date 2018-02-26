package io.treehopper.libraries.sensors;

public abstract class ProximitySensor implements IPollable {
    protected double meters;
    private boolean autoUpdateWhenPropertyRead = true;

    @Override
    public boolean isAutoUpdateWhenPropertyRead() {
        return autoUpdateWhenPropertyRead;
    }

    @Override
    public void setAutoUpdateWhenPropertyRead(boolean value) {
        autoUpdateWhenPropertyRead = value;
    }

    public double getMeters() {
        if (isAutoUpdateWhenPropertyRead())
            update();

        return meters;
    }

    public double getCentimeters() {
        return getMeters() * 100;
    }

    public double getMillimeters() {
        return getMeters() * 1000;
    }

    public double getInches() {
        return getMeters() * 39.3701;
    }

    public double getFeet() {
        return getMeters() * 3.28085;
    }
}
