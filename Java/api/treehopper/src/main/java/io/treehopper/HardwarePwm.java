package io.treehopper;

import io.treehopper.enums.PinMode;
import io.treehopper.interfaces.Pwm;

/**
 * Hardware PWM pin
 */
public class HardwarePwm implements Pwm {
    Pin pin;
    TreehopperUsb board;
    boolean enabled = false;
    double dutyCycle = 0.0;

    HardwarePwm(Pin pin) {
        this.pin = pin;
        this.board = pin.getBoard();
    }

    /**
     * Gets whether this PWM pin is enabled
     *
     * @return whether this PWM pin is enabled
     */
    @Override
    public boolean isEnabled() {
        return enabled;
    }

    /**
     * Sets whether this PWM pin is enabled
     *
     * @param enabled whether this PWM pin is enabled
     */
    @Override
    public void setEnabled(boolean enabled) {
        if (this.enabled == enabled) return;

        this.enabled = enabled;

        if (enabled) {
            board.hardwarePwmManager.startPin(pin);
            pin.setMode(PinMode.Reserved);
        } else {
            board.hardwarePwmManager.stopPin(pin);
            pin.setMode(PinMode.Unassigned);
        }
    }

    /**
     * Gets the pulse width of the pin
     *
     * @return the pulse width, in microseconds
     */
    @Override
    public double getPulseWidth() {
        return dutyCycle * board.hardwarePwmManager.getPeriodMicroseconds();
    }

    /**
     * Sets the pulse width of the pin
     *
     * @param pulseWidth the pulse width, in microseconds
     */
    @Override
    public void setPulseWidth(double pulseWidth) {
        if (Utilities.CloseTo(pulseWidth, getPulseWidth())) return;

        setDutyCycle(pulseWidth / board.hardwarePwmManager.getPeriodMicroseconds());
    }

    /**
     * Gets the duty cycle of the pin
     *
     * @return the duty cycle, 0-1.
     */
    @Override
    public double getDutyCycle() {
        return dutyCycle;
    }

    /**
     * Sets the duty cycle of the pin
     *
     * @param dutyCycle the duty cycle, 0-1.
     */
    @Override
    public void setDutyCycle(double dutyCycle) {
        if (Utilities.CloseTo(dutyCycle, this.dutyCycle)) return;

        this.dutyCycle = dutyCycle;
        board.hardwarePwmManager.setDutyCycle(pin, dutyCycle);
    }
}
