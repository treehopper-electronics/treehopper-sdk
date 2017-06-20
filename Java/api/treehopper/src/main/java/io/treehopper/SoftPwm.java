package io.treehopper;

import io.treehopper.enums.PinMode;
import io.treehopper.interfaces.Pwm;

/**
     This class provides software-based pulse-width modulation (PWM) on any pin.

         The period of the SoftPwm module defaults to approximately 60 Hz. Changing this period will affect all active
         SoftPwm pins.

         Compared to HardwarePwm (which is implemented in hardware), SoftPwm has some jitter.
         However, it has good precision, fine-tuned period control, and works well even when many (or all!) Treehopper pins are used for SoftPwm.
*/
public class SoftPwm implements Pwm {

    private final Pin _pin;
    private final TreehopperUsb _board;
    boolean _isEnabled;

    public SoftPwm(TreehopperUsb board, Pin pin)
    {
        this._board = board;
        this._pin = pin;
    }

    @Override
    public double getDutyCycle() {
        return _board.softPwmManager.getDutyCycle(_pin);
    }

    @Override
    public void setDutyCycle(double dutyCycle) {
        _board.softPwmManager.setDutyCycle(_pin, dutyCycle);
    }

    @Override
    public double getPulseWidth() {
        return _board.softPwmManager.getPulseWidth(_pin);
    }

    @Override
    public void setPulseWidth(double pulseWidth) {
        _board.softPwmManager.setPulseWidth(_pin, pulseWidth);
    }

    @Override
    public boolean isEnabled() {
        return _isEnabled;
    }

    @Override
    public void setEnabled(boolean enabled) {
        if(enabled == _isEnabled) return; // no change

        _isEnabled = enabled;

        if (_isEnabled)
        {
            _board.softPwmManager.StartPin(_pin);
            _pin.setMode(PinMode.PushPullOutput);
        }
        else
        {
            _board.softPwmManager.StopPin(_pin);
            _pin.setMode(PinMode.DigitalInput);
        }
    }

    @Override
    public String toString() {
        return isEnabled() ? String.format("%f%% duty cycle (%f us pulse width)", getDutyCycle() * 100, getPulseWidth()) : "Not enabled";
    }
}
