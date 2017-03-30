package io.treehopper.events;

/**
 * ADC value changed EventArgs
 */
public class AdcValueChangedEventArgs {
    public int newAdcValue;
    public int oldAdcValue;

    /**
     * Construct a new AdcValueChangedEventArgs
     * @param newAdcValue the new ADC value
     * @param oldAdcValue the previous ADC value
     */
    public AdcValueChangedEventArgs(int newAdcValue, int oldAdcValue) {
        this.newAdcValue = newAdcValue;
        this.oldAdcValue = oldAdcValue;
    }
}
