package io.treehopper.libraries.displays;

/**
 * LED
 */

public class Led {

    private ILedDriver driver;
    private boolean brightnessControl;
    int channel;
    private boolean state = false;
    private double brightness = 0.0;

    public Led(ILedDriver driver, int channel) {
        this(driver, channel, false);
    }

    public Led(ILedDriver driver, int channel, boolean hasBrightnessControl) {
        this.driver = driver;
        this.brightnessControl = hasBrightnessControl;
        this.channel = channel;
    }

    public int getChannel() {
        return channel;
    }

    public ILedDriver getDriver() {
        return driver;
    }

    public double getBrightness() {
        return brightness;
    }
    

    public void setBrightness(double value) {
        if (brightness == value) return;
        if (!hasBrightnessControl()) return;
        brightness = value;
        driver.ledBrightnessChanged(this);
    }


    public boolean hasBrightnessControl() {
        return brightnessControl;
    }


    public boolean getState() {
        return state;
    }

    public void setState(boolean value) {
        if (state == value) return;
        state = value;
        driver.ledStateChanged(this);
    }
}
