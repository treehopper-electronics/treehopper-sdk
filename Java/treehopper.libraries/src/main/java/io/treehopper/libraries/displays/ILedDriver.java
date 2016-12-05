package io.treehopper.libraries.displays;

import java.util.ArrayList;
import java.util.List;

import io.treehopper.libraries.io.IFlushable;

/**
 * Created by jay on 12/5/2016.
 */

public interface ILedDriver extends IFlushable {
    boolean hasGlobalBrightnessControl();
    boolean hasIndividualBrightnessControl();
    double getBrightness();
    void setBrightness(double brightness);
    List<Led> leds = new ArrayList<Led>();

    void ledStateChanged(Led led);
    void ledBrightnessChanged(Led led);
    void clear();
}
