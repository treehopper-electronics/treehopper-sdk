package io.treehopper.libraries.displays;

import io.treehopper.libraries.IFlushable;

import java.util.ArrayList;
import java.util.List;

/**
 * LED driver interface
 */
public interface ILedDriver extends IFlushable {
    List<Led> leds = new ArrayList<Led>();

    boolean hasGlobalBrightnessControl();

    boolean hasIndividualBrightnessControl();

    double getBrightness();

    void setBrightness(double brightness);

    void ledStateChanged(Led led);

    void ledBrightnessChanged(Led led);

    void clear();
}
