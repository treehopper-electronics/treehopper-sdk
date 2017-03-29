package io.treehopper;

import java.util.ArrayList;
import java.util.List;

import io.treehopper.enums.AdcReferenceLevel;
import io.treehopper.events.AdcValueChangedEventArgs;
import io.treehopper.events.AdcValueChangedEventHandler;
import io.treehopper.events.AnalogValueChangedEventArgs;
import io.treehopper.events.AnalogValueChangedEventHandler;
import io.treehopper.events.AnalogVoltageChangedEventArgs;
import io.treehopper.events.AnalogVoltageChangedEventHandler;
import io.treehopper.events.DigitalInValueChangedEventArgs;
import io.treehopper.events.DigitalInValueChangedEventHandler;
import io.treehopper.enums.PinMode;
import io.treehopper.interfaces.DigitalIOPin;

import static java.lang.Math.abs;

/**
 * Created by jay on 12/28/2015.
 */

public class Pin implements DigitalIOPin {
    private TreehopperUsb board;

    public int getPinNumber() {
        return pinNumber;
    }

    private int pinNumber;

    // digital members
    private boolean digitalValue;
    private PinMode mode = PinMode.Unassigned;

    // analog members
    int adcValueChangedThreshold = 2;
    double analogVoltageChangedThreshold = 0.1;
    AdcReferenceLevel referenceLevel = AdcReferenceLevel.VREF_3V3;

    double analogValueChangedThreshold = 0.05;
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

    public TreehopperUsb getBoard() {
        return this.board;
    }

    public void setMode(PinMode value) {
        if (value == mode)
            return;
        if (mode == PinMode.Reserved && value != PinMode.Unassigned) {
            throw new RuntimeException("This pin is reserved; you must disable the peripheral using it before interacting with it");
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

    //// DIGITAL

    private List<DigitalInValueChangedEventHandler> digitalInValueListeners = new ArrayList<>();

    public void addDigitalInValueChangedEventHandler(DigitalInValueChangedEventHandler handler) {
        digitalInValueListeners.add(handler);
    }

    public void removeDigitalInValueChangedEventHandler(DigitalInValueChangedEventHandler handler) {
        digitalInValueListeners.remove(handler);
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

    @Override
    public void makeDigitalPushPullOutput() {
        setMode(PinMode.PushPullOutput);
    }

    @Override
    public void makeDigitalIn() {
        setMode(PinMode.DigitalInput);
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
            }
        } else if(mode == PinMode.AnalogInput)
        {
            adcValue = (highByte & 0xFF) << 8 | (lowByte & 0xFF);
            RaiseAnalogInChanged();

        }
    }

    void RaiseDigitalInValueChanged()
    {
        DigitalInValueChangedEventArgs eventArgs = new DigitalInValueChangedEventArgs(digitalValue);
        for(DigitalInValueChangedEventHandler handler : digitalInValueListeners)
            handler.DigitalValueChanged(this, eventArgs);
    }

//// ANALOG

    private List<AdcValueChangedEventHandler> adcValueListeners = new ArrayList<>();
    private List<AnalogValueChangedEventHandler> analogValueListeners = new ArrayList<>();
    private List<AnalogVoltageChangedEventHandler> analogVoltageListeners = new ArrayList<>();

    public void addAdcValueChangedEventHandler(AdcValueChangedEventHandler handler) {
        adcValueListeners.add(handler);
    }

    public void removeAdcValueChangedEventHandler(AdcValueChangedEventHandler handler) {
        adcValueListeners.remove(handler);
    }

    public void addAnalogValueChangedEventHandler(AnalogValueChangedEventHandler handler) {
        analogValueListeners.add(handler);
    }

    public void removeAnalogValueChangedEventHandler(AnalogValueChangedEventHandler handler) {
        analogValueListeners.remove(handler);
    }

    public void addAnalogVoltageChangedEventHandler(AnalogVoltageChangedEventHandler handler) {
        analogVoltageListeners.add(handler);
    }

    public void removeAnalogVoltageChangedEventHandler(AnalogVoltageChangedEventHandler handler) {
        analogVoltageListeners.remove(handler);
    }

    public int getAdcValueChangedThreshold() {
        return adcValueChangedThreshold;
    }

    public void setAdcValueChangedThreshold(int adcValueChangedThreshold) {
        this.adcValueChangedThreshold = adcValueChangedThreshold;
    }


    public double getAnalogValueChangedThreshold() {
        return analogValueChangedThreshold;
    }

    public void setAnalogValueChangedThreshold(double analogValueChangedThreshold) {
        this.analogValueChangedThreshold = analogValueChangedThreshold;
    }


    public double getAnalogVoltageChangedThreshold() {
        return analogVoltageChangedThreshold;
    }

    public void setAnalogVoltageChangedThreshold(double analogVoltageChangedThreshold) {
        this.analogVoltageChangedThreshold = analogVoltageChangedThreshold;
    }

    void RaiseAnalogInChanged()
    {
        if (abs(prevAdcValue - adcValue) > adcValueChangedThreshold)
        {
            AdcValueChangedEventArgs eventArgs = new AdcValueChangedEventArgs(adcValue, prevAdcValue);
            for(AdcValueChangedEventHandler handler : adcValueListeners)
                handler.analogValueChanged(this, eventArgs);

            prevAdcValue = adcValue;
        }

        if (abs(prevAnalogVoltage - getAnalogVoltage()) > analogVoltageChangedThreshold)
        {
            AnalogVoltageChangedEventArgs eventArgs = new AnalogVoltageChangedEventArgs(getAnalogVoltage(), prevAnalogVoltage);
            for(AnalogVoltageChangedEventHandler handler: analogVoltageListeners)
                handler.analogVoltageChanged(this, eventArgs);

            prevAnalogVoltage = getAnalogVoltage();
        }

        if(abs(prevAnalogValue - getAnalogValue()) > analogValueChangedThreshold)
        {
            AnalogValueChangedEventArgs eventArgs = new AnalogValueChangedEventArgs(getAnalogValue(), prevAnalogValue);
            for(AnalogValueChangedEventHandler handler: analogValueListeners)
                handler.analogValueChanged(this, eventArgs);

            prevAnalogValue = getAnalogValue();
        }
    }


    public int getAdcValue()
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


    public AdcReferenceLevel getReferenceLevel() {
        return referenceLevel;
    }

    public void setReferenceLevel(AdcReferenceLevel referenceLevel) {
        this.referenceLevel = referenceLevel;
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
	Reserved,
    MakeDigitalInput,
    MakePushPullOutput,
    MakeOpenDrainOutput,
    MakeAnalogInput,
    SetDigitalValue,
    GetDigitalValue,
    GetAnalogValue
}