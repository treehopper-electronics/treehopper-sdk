package io.treehopper.libraries.displays;

import io.treehopper.libraries.io.IFlushable;
import java.util.ArrayList;
import java.util.List;

/**
 * Created by jay on 12/5/2016.
 */

public abstract class LedDriver implements ILedDriver, IFlushable {


    private boolean autoFlush;
    private double brightness;
    private List<Led> leds;
    


    public LedDriver(int numLeds, boolean HasGlobalBrightnessControl, boolean HasIndividualBrightnessControl)
    {
    	this.autoFlush = true;
    	this.brightness = 1.0;
    	this.leds = new ArrayList<>();
        for (int i = 0; i < numLeds; i++)
        {
            Led led = new Led(this, i, HasIndividualBrightnessControl);
            this.leds.add(led);
        }
    }

    public List<Led> getLeds(){
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

    @Override
    public double getBrightness(){
    	return brightness;
    }

    @Override
    public void setBrightness(double brightness){
        if (Math.abs(this.brightness - brightness) < 0.0005) return;
        if (this.brightness < 0 || brightness > 1)
			try {
				throw new Exception("Valid brightness is from 0 to 1");
			} catch (Exception e) {

			}
        this.brightness = brightness;

        setBrightness(brightness);
    }

    public abstract void ledStateChanged(Led led);
    public abstract void ledBrightnessChanged(Led led);

    public void clear()
    {
        boolean autoflushSave = isAutoFlushEnabled();
        setAutoFlushEnabled(false);
        for(Led led : this.leds)
        {
            led.setState(false);
        }
        flush(true);
        setAutoFlushEnabled(autoflushSave);
    }

    public abstract void flush(boolean force);

}
