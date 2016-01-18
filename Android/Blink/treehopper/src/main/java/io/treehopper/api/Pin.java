package io.treehopper.api;

import java.util.Arrays;

/**
 * Created by jay on 12/28/2015.
 */

public class Pin {
    private TreehopperUsb board;
    private int pinNumber;
    private String ioName;

    // digital members
    private boolean digitalValue;
    private PinMode mode;

    // analog members
    int adcValueChangedThreshold = 2;
    double adcVoltageChangedThreshold = 0.05;
    private AdcReferenceLevel referenceLevel = AdcReferenceLevel.VREF_3V3;

    public Pin(TreehopperUsb board, int pinNumber, String ioName) {
        this.board = board;
        this.pinNumber = pinNumber;
        this.ioName = ioName;
    }

    public void setMode(PinMode value) {
        if (value == mode)
            return;
        if (mode == PinMode.Reserved && value != PinMode.Unassigned) {
            // throw new Exception("This pin is reserved; you must disable the peripheral using it before interacting with it");
            return;
        }

        mode = value;

        switch (mode) {
            case AnalogInput:
                sendCommand(new byte[]{(byte) PinConfigCommands.MakeAnalogInput.ordinal(), (byte) referenceLevel.ordinal()});
                break;
            case DigitalInput:
                sendCommand(new byte[]{(byte) PinConfigCommands.MakeDigitalInput.ordinal(), 0x00});
                break;
            case OpenDrainOutput:
                sendCommand(new byte[]{(byte) PinConfigCommands.MakeOpenDrainOutput.ordinal(), 0x00});
                break;
            case PushPullOutput:
                sendCommand(new byte[]{(byte) PinConfigCommands.MakePushPullOutput.ordinal(), 0x00});
                break;
        }
    }

    public PinMode getMode() {
        return mode;
    }

    public void setDigitalValue(boolean value) {
        digitalValue = value;
        if (!(mode == PinMode.PushPullOutput || mode == PinMode.OpenDrainOutput))
            mode = PinMode.PushPullOutput; // assume they want push-pull
        byte byteVal = (byte) (digitalValue ? 0x01 : 0x00);
        sendCommand(new byte[]{(byte) PinConfigCommands.SetDigitalValue.ordinal(), byteVal});
    }

    public boolean getDigitalValue() {
        return digitalValue;
    }

    public void toggleOutput() {
        setDigitalValue(!getDigitalValue());
    }

    private void sendCommand(byte[] cmd) {
        byte[] data = new byte[8];
        data[0] = (byte) DeviceCommands.PinConfig.ordinal();
        data[1] = (byte) pinNumber;
        System.arraycopy(cmd, 0, data, 2, cmd.length);
        board.getConnection().sendDataPinConfigChannel(data);
    }
}


enum PinConfigCommands {
    MakeDigitalInput,
    MakePushPullOutput,
    MakeOpenDrainOutput,
    MakeAnalogInput,
    SetDigitalValue,
    GetDigitalValue,
    GetAnalogValue
}