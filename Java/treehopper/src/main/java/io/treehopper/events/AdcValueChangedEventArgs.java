package io.treehopper.events;

/**
 * Created by jay on 12/6/2016.
 */
public class AdcValueChangedEventArgs {
    public AdcValueChangedEventArgs(int newAdcValue, int oldAdcValue) {
        this.newAdcValue = newAdcValue;
        this.oldAdcValue = oldAdcValue;
    }

    public int newAdcValue;
    public int oldAdcValue;
}
