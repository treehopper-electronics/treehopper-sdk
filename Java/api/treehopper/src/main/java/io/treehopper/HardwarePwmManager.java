package io.treehopper;

import io.treehopper.enums.HardwarePwmFrequency;

enum PwmPinEnableMode {
    None,
    Pin7,
    Pin7_Pin8,
    Pin7_Pin8_Pin9
}

/**
 * Manager class for the hardware PWM pins
 */
public class HardwarePwmManager {
    private TreehopperUsb board;
    private PwmPinEnableMode mode;
    private byte[] dutyCyclePin7 = new byte[2];
    private byte[] dutyCyclePin8 = new byte[2];
    private byte[] dutyCyclePin9 = new byte[2];
    private HardwarePwmFrequency frequency = HardwarePwmFrequency.Freq_732Hz;

    HardwarePwmManager(TreehopperUsb treehopperUSB) {
        this.board = treehopperUSB;
    }

    public double getMicrosecondsPerTick() {
        return 1000000 / (getFrequencyHz() * 65536);
    }

    public double getPeriodMicroseconds() {
        return 1000000 / getFrequencyHz();
    }

    public int getFrequencyHz() {
        return frequency.getFrequencyHz();
    }

    void startPin(Pin pin) {
        // first check to make sure the previous PWM pins have been enabled first.
        if (pin.getPinNumber() == 8 & mode != PwmPinEnableMode.Pin7)
            throw new RuntimeException("You must enable PWM functionality on Pin 8 (PWM1) before you enable PWM functionality on Pin 9 (PWM2). See http://treehopper.io/pwm");
        if (pin.getPinNumber() == 9 & mode != PwmPinEnableMode.Pin7_Pin8)
            throw new RuntimeException("You must enable PWM functionality on Pin 8 and 9 (PWM1 and PWM2) before you enable PWM functionality on Pin 10 (PWM3). See http://treehopper.io/pwm");

        switch (pin.getPinNumber()) {
            case 7:
                mode = PwmPinEnableMode.Pin7;
                break;
            case 8:
                mode = PwmPinEnableMode.Pin7_Pin8;
                break;
            case 9:
                mode = PwmPinEnableMode.Pin7_Pin8_Pin9;
                break;
        }

        sendConfig();

    }


    void stopPin(Pin pin) {
        // first check to make sure the higher PWM pins have been disabled first
        if (pin.getPinNumber() == 8 & mode != PwmPinEnableMode.Pin7_Pin8)
            throw new RuntimeException("You must disable PWM functionality on Pin 10 (PWM3) before disabling Pin 9's PWM functionality. See http://treehopper.io/pwm");
        if (pin.getPinNumber() == 7 & mode != PwmPinEnableMode.Pin7)
            throw new RuntimeException("You must disable PWM functionality on Pin 9 and 10 (PWM2 and PWM3) before disabling Pin 8's PWM functionality. See http://treehopper.io/pwm");

        switch (pin.getPinNumber()) {
            case 7:
                mode = PwmPinEnableMode.None;
                break;
            case 8:
                mode = PwmPinEnableMode.Pin7;
                break;
            case 9:
                mode = PwmPinEnableMode.Pin7_Pin8;
                break;
        }

        sendConfig();
    }

    void setDutyCycle(Pin pin, double value) {
        int PwmRegisterValue = (int) Math.round(value * 65535.0);
        byte[] newValue = new byte[2];

        newValue[0] = (byte) (PwmRegisterValue & 0xff);
        newValue[1] = (byte) ((PwmRegisterValue >> 8) & 0xff);

        switch (pin.getPinNumber()) {
            case 7:
                dutyCyclePin7 = newValue;
                break;
            case 8:
                dutyCyclePin8 = newValue;
                break;
            case 9:
                dutyCyclePin9 = newValue;
                break;
        }

        sendConfig();
    }

    private void sendConfig() {
        byte[] configuration = new byte[64];

        configuration[0] = (byte) DeviceCommands.PwmConfig.ordinal();
        configuration[1] = (byte) mode.ordinal();
        configuration[2] = (byte) frequency.ordinal();

        configuration[3] = dutyCyclePin7[0];
        configuration[4] = dutyCyclePin7[1];

        configuration[5] = dutyCyclePin8[0];
        configuration[6] = dutyCyclePin8[1];

        configuration[7] = dutyCyclePin9[0];
        configuration[8] = dutyCyclePin9[1];

        board.sendPeripheralConfigPacket(configuration);
    }
};