package io.treehopper.interfaces;

/**
 * PWM pin
 */
public interface Pwm {
    public double getDutyCycle();

    public void setDutyCycle(double dutyCycle);

    public double getPulseWidth();

    public void setPulseWidth(double pulseWidth);

    public boolean isEnabled();

    public void setEnabled(boolean enabled);

}
