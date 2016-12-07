package io.treehopper;

import io.treehopper.enums.PinMode;
import io.treehopper.interfaces.Pwm;

/**
 * Created by jay on 12/6/2016.
 */

public class HardwarePwm implements Pwm {
    public HardwarePwm(Pin pin) {
        this.pin = pin;
        this.board = pin.getBoard();
    }

    Pin pin;
    TreehopperUsb board;
    boolean enabled = false;
    double pulseWidth = 0.0;
    double dutyCycle = 0.0;

    @Override
    public boolean isEnabled() {
        return enabled;
    }

    @Override
    public void setEnabled(boolean enabled) {
        if(this.enabled == enabled) return;

        this.enabled = enabled;

        if (enabled)
        {
            board.hardwarePwmManager.startPin(pin);
            pin.setMode(PinMode.Reserved);
        }
        else
        {
            board.hardwarePwmManager.stopPin(pin);
            pin.setMode(PinMode.Unassigned);
        }
    }

    @Override
    public double getPulseWidth() {
        return pulseWidth;
    }

    @Override
    public void setPulseWidth(double pulseWidth) {
        if(Math.abs(this.pulseWidth - pulseWidth) < 0.001) return;

        this.pulseWidth = pulseWidth;
    }

    @Override
    public double getDutyCycle() {
        return dutyCycle;
    }

    @Override
    public void setDutyCycle(double dutyCycle) {
        if(Math.abs(this.dutyCycle - dutyCycle) < 0.001) return;

        this.dutyCycle = dutyCycle;

        board.hardwarePwmManager.setDutyCycle(pin, dutyCycle);
    }
}
