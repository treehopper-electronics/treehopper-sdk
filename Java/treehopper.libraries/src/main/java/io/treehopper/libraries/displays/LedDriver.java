package io.treehopper.libraries.displays;

import io.treehopper.libraries.io.IFlushable;
import java.util.ArrayList;
import java.util.List;

/**
 * Created by jay on 12/5/2016.
 */

public abstract class LedDriver implements ILedDriver, IFlushable {


    private boolean autoFlush = true;


    public LedDriver(int numLeds, boolean HasGlobalBrightnessControl, boolean HasIndividualBrightnessControl)
    {
        for (int i = 0; i < numLeds; i++)
        {
            Led led = new Led(this, i, HasIndividualBrightnessControl);
            leds.add(led);
        }
    }

    @Override
    public boolean isAutoFlushEnabled() {
        return autoFlush;
    }

    public void setAutoFlushEnabled(boolean autoFlush) {
        this.autoFlush = autoFlush;
    }

    public List<Led> leds  = new ArrayList<>();


    @Override
    public abstract boolean hasGlobalBrightnessControl();

    @Override
    public abstract boolean hasIndividualBrightnessControl();

    @Override
    public abstract double getBrightness();

    @Override
    public abstract void setBrightness(double brightness);

    public abstract void ledStateChanged(Led led);
    public abstract void ledBrightnessChanged(Led led);

    public void clear()
    {
        boolean autoflushSave = isAutoFlushEnabled();
        setAutoFlushEnabled(false);
        for(Led led : leds)
        {
            led.setState(false);
        }
        flush(true);
        setAutoFlushEnabled(autoflushSave);
    }

    public abstract void flush(boolean force);

}
