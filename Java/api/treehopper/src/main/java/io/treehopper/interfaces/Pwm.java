package io.treehopper.interfaces;

/**
 * Created by jay on 12/28/2015.
 */
public interface Pwm {
    public double getDutyCycle();
    public void setDutyCycle(double dutyCycle);
    public double getPulseWidth();
    public void setPulseWidth(double pulseWidth);
    public boolean isEnabled();
    public void setEnabled(boolean enabled);

}
