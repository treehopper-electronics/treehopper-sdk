package io.treehopper.libraries.displays;

import io.treehopper.libraries.IFlushable;

import java.util.ArrayList;
import java.util.List;

/**
 * Base ILedDriver implementation
 */
public abstract class LedDriver implements ILedDriver, IFlushable {


    private boolean autoFlush;
    private double brightness;
    private List<Led> leds;


    public LedDriver(int numLeds, boolean HasGlobalBrightnessControl, boolean HasIndividualBrightnessControl) {
        this.autoFlush = true;
        this.brightness = 0.0;
        this.leds = new ArrayList<>();
        for (int i = 0; i < numLeds; i++) {
            Led led = new Led(this, i, HasIndividualBrightnessControl);
            this.leds.add(led);
        }
    }

    public List<Led> getLeds() {
        return leds;
    }

    @Override
    public boolean isAutoFlushEnabled() {
        return autoFlush;
    }

    public void setAutoFlushEnabled(boolean autoFlush) {
        this.autoFlush = autoFlush;
    }


    @Override
    public abstract boolean hasGlobalBrightnessControl();

    @Override
    public abstract boolean hasIndividualBrightnessControl();

    public double getBrightness() {
        return brightness;
    }

    public void setBrightness(double brightness) {
        if (Math.abs(this.brightness - brightness) < 0.0005) return;
        if (this.brightness < 0 || brightness > 1)
            throw new RuntimeException("Valid brightness is from 0 to 1");

        this.brightness = brightness;

        _setBrightness(brightness);
    }

    protected abstract void _setBrightness(double brightness);

    public abstract void ledStateChanged(Led led);

    public abstract void ledBrightnessChanged(Led led);

    public void clear() {
        boolean autoflushSave = isAutoFlushEnabled();
        setAutoFlushEnabled(false);
        for (Led led : this.leds) {
            led.setState(false);
        }
        flush(true);
        setAutoFlushEnabled(autoflushSave);
    }

    public abstract void flush(boolean force);

}
