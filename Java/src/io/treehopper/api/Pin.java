package io.treehopper.api;

import java.util.Arrays;
import java.util.Observable;

import static java.lang.Math.abs;

/**
 * Created by jay on 12/28/2015.
 */

public class Pin extends Observable {
    private TreehopperUsb board;
    private int pinNumber;

    // digital members
    private boolean digitalValue;
    private PinMode mode = PinMode.Unassigned;

    // analog members
    int adcValueChangedThreshold = 2;
    double analogValueChangedThreshold = 0.05;
    double analogVoltageChangedThreshold = 0.1;
    AdcReferenceLevel referenceLevel = AdcReferenceLevel.VREF_3V3;

    int adcValue;
    double analogVoltage;
    double analogValue;

    private int prevAdcValue;
    double prevAnalogVoltage;
    double prevAnalogValue;


    public Pin(TreehopperUsb board, int pinNumber)  {
        this.board = board;
        this.pinNumber = pinNumber;
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
        byte[] data = new byte[6];
        data[0] = (byte) pinNumber;
        System.arraycopy(cmd, 0, data, 1, cmd.length);
        board.getConnection().sendDataPinConfigChannel(data);
    }

    public void UpdateValue(byte highByte, byte lowByte) {
        if(mode == PinMode.DigitalInput)
        {
            boolean newVal = highByte > 0;
            if (digitalValue != newVal) // we have a new value!
            {
                digitalValue = newVal;

                RaiseDigitalInValueChanged();

                // TODO: How do I do this in Java?
                // RaisePropertyChanged("DigitalValue");
            }
        } else if(mode == PinMode.AnalogInput)
        {
            adcValue = (highByte & 0xFF) << 7 | (lowByte & 0xFF) >> 1;
            RaiseAnalogInChanged();

        }
    }

    void RaiseDigitalInValueChanged()
    {
        // TODO: how do I do this in Java?
        // DigitalValueChanged?.Invoke(this, digitalValue);
    }

    void RaiseAnalogInChanged()
    {
        if (abs(prevAdcValue - adcValue) > adcValueChangedThreshold)
        {
            prevAdcValue = adcValue;
            // TODO
            // AdcValueChanged?.Invoke(this, adcValue);
            // RaisePropertyChanged("AdcValue");
        }

        if (abs(prevAnalogVoltage - getAnalogVoltage()) > analogVoltageChangedThreshold)
        {
            prevAnalogVoltage = getAnalogVoltage();
            // TODO
            // AnalogVoltageChanged?.Invoke(this, AnalogVoltage);
            // RaisePropertyChanged("AnalogVoltage");
        }

        if(abs(prevAnalogValue - getAnalogValue()) > analogValueChangedThreshold)
        {
            prevAnalogValue = getAnalogValue();
            // TODO
            // AnalogValueChanged?.Invoke(this, AnalogValue);
            // RaisePropertyChanged("AnalogValue");
        }
    }

    public double getAdcValue()
    {
        return adcValue;
    }

    public double getAnalogVoltage()
    {
        return adcValue * (getReferenceLevelVoltage() / 4092.0);
    }

    public double getAnalogValue()
    {
        return adcValue / 4092.0;
    }

    double getReferenceLevelVoltage()
    {
        switch(referenceLevel)
        {
            case VREF_1V65:
                return 1.65;

            case VREF_1V8:
                return 1.8;

            case VREF_2V4:
                return 2.4;

            case VREF_3V3:
                return 3.6;

            case VREF_3V3_DERIVED:
                return 3.3;

            case VREF_3V6:
                return 3.6;

            default:
                return 0;
        }
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