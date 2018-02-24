package io.treehopper;

import java.util.ArrayList;
import java.util.List;

import io.treehopper.enums.AdcReferenceLevel;
import io.treehopper.enums.PinMode;
import io.treehopper.events.AdcValueChangedEventArgs;
import io.treehopper.events.AdcValueChangedEventHandler;
import io.treehopper.events.AnalogValueChangedEventArgs;
import io.treehopper.events.AnalogValueChangedEventHandler;
import io.treehopper.events.AnalogVoltageChangedEventArgs;
import io.treehopper.events.AnalogVoltageChangedEventHandler;
import io.treehopper.events.DigitalInValueChangedEventArgs;
import io.treehopper.events.DigitalInValueChangedEventHandler;
import io.treehopper.interfaces.DigitalIO;
import io.treehopper.interfaces.Spi;
import io.treehopper.interfaces.SpiChipSelectPin;

import static java.lang.Math.abs;

enum PinConfigCommands {
    Reserved,
    MakeDigitalInput,
    MakePushPullOutput,
    MakeOpenDrainOutput,
    MakeAnalogInput,
    SetDigitalValue
}

/**
 * A GPIO/Analog pin on the board
 */
public class Pin implements DigitalIO, SpiChipSelectPin {
    public final SoftPwm softPwm;
    // analog members
    int adcValueChangedThreshold = 2;
    double analogVoltageChangedThreshold = 0.1;
    AdcReferenceLevel referenceLevel = AdcReferenceLevel.VREF_3V3;
    double analogValueChangedThreshold = 0.05;
    int adcValue;
    double analogVoltage;
    double analogValue;
    double prevAnalogVoltage;
    double prevAnalogValue;
    private TreehopperUsb board;
    private int pinNumber;
    // digital members
    private boolean digitalValue;
    private PinMode mode = PinMode.Unassigned;
    private int prevAdcValue;
    private List<DigitalInValueChangedEventHandler> digitalInValueListeners = new ArrayList<>();
    private List<AdcValueChangedEventHandler> adcValueListeners = new ArrayList<>();
    private List<AnalogValueChangedEventHandler> analogValueListeners = new ArrayList<>();
    private List<AnalogVoltageChangedEventHandler> analogVoltageListeners = new ArrayList<>();

    Pin(TreehopperUsb board, int pinNumber) {
        this.board = board;
        this.pinNumber = pinNumber;
        this.softPwm = new SoftPwm(board, this);
    }

    //// DIGITAL

    /**
     * Gets the pin number of this pin
     *
     * @return the pin number (starting at 0)
     */
    public int getPinNumber() {
        return pinNumber;
    }

    /**
     * Gets the SPI module that can use this pin as an SpiChipSelectPin
     *
     * @return The supported SPI module
     */
    @Override
    public Spi getSpiModule() {
        return board.spi;
    }

    /**
     * Get the board this pin belongs to
     *
     * @return the board this pin belongs to
     */
    public TreehopperUsb getBoard() {
        return this.board;
    }

    /**
     * Get the current PinMode of the pin
     *
     * @return the PinMode of the pin
     */
    public PinMode getMode() {
        return mode;
    }

    /**
     * Set this pin's mode
     *
     * @param value the mode of the pin
     */
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
                digitalValue = false; // set initial state
                break;
            case PushPullOutput:
                sendCommand(new byte[]{(byte) PinConfigCommands.MakePushPullOutput.ordinal(), 0x00});
                digitalValue = false; // set initial state
                break;
        }
    }

    /**
     * Add a DigitalInValueCHangedEventHandler
     *
     * @param handler the handler to add
     */
    public void addDigitalInValueChangedEventHandler(DigitalInValueChangedEventHandler handler) {
        digitalInValueListeners.add(handler);
    }

    /**
     * Remove a DigitalInValueCHangedEventHandler
     *
     * @param handler the handler to remove
     */
    public void removeDigitalInValueChangedEventHandler(DigitalInValueChangedEventHandler handler) {
        digitalInValueListeners.remove(handler);
    }

    /**
     * Gets the current digital value of the pin
     *
     * @return the digital value of the pin
     * <p>
     * If the pin is an input, this function returns the last-read value of the pin. If the pin is an output, this function returns the value this pin is currently outputting.
     */
    public boolean getDigitalValue() {
        return digitalValue;
    }

    /**
     * Sets the digital value of this pin
     *
     * @param value the digital value
     */
    public void setDigitalValue(boolean value) {
        digitalValue = value;
        if (!(mode == PinMode.PushPullOutput || mode == PinMode.OpenDrainOutput))
            mode = PinMode.PushPullOutput; // assume they want push-pull
        byte byteVal = (byte) (digitalValue ? 0x01 : 0x00);
        sendCommand(new byte[]{(byte) PinConfigCommands.SetDigitalValue.ordinal(), byteVal});
    }

    /**
     * Toggles the output of the pin.
     * <p>
     * Just like setDigitalValue(), this function will turn the pin into a push-pull output if it is not already a push-pull or open-drain output.
     */
    public void toggleOutput() {
        setDigitalValue(!getDigitalValue());
    }

    /**
     * Make this pin a digital push-pull output
     */
    @Override
    public void makeDigitalPushPullOutput() {
        setMode(PinMode.PushPullOutput);
    }

    /**
     * Make this pin a digital input
     */
    @Override
    public void makeDigitalIn() {
        setMode(PinMode.DigitalInput);
    }

//// ANALOG

    private void sendCommand(byte[] cmd) {
        byte[] data = new byte[6];
        data[0] = (byte) pinNumber;
        System.arraycopy(cmd, 0, data, 1, cmd.length);
        board.getConnection().sendDataPinConfigChannel(data);
    }

    void UpdateValue(byte highByte, byte lowByte) {
        if (mode == PinMode.DigitalInput) {
            boolean newVal = highByte > 0;
            if (digitalValue != newVal) // we have a new value!
            {
                digitalValue = newVal;

                RaiseDigitalInValueChanged();
            }
        } else if (mode == PinMode.AnalogInput) {
            adcValue = (highByte & 0xFF) << 8 | (lowByte & 0xFF);
            RaiseAnalogInChanged();

        }
    }

    void RaiseDigitalInValueChanged() {
        DigitalInValueChangedEventArgs eventArgs = new DigitalInValueChangedEventArgs(digitalValue);
        for (DigitalInValueChangedEventHandler handler : digitalInValueListeners)
            handler.DigitalValueChanged(this, eventArgs);
    }

    /**
     * Add an AdcValueChangedEventHandler
     *
     * @param handler the AdcValueChangedEventHandler to add
     */
    public void addAdcValueChangedEventHandler(AdcValueChangedEventHandler handler) {
        adcValueListeners.add(handler);
    }

    /**
     * Remove an AdcValueChangedEventHandler
     *
     * @param handler the AdcValueChangedEventHandler to remove
     */
    public void removeAdcValueChangedEventHandler(AdcValueChangedEventHandler handler) {
        adcValueListeners.remove(handler);
    }

    /**
     * Add an AnalogValueChangedEventHandler
     *
     * @param handler the AnalogValueChangedEventHandler to add
     */
    public void addAnalogValueChangedEventHandler(AnalogValueChangedEventHandler handler) {
        analogValueListeners.add(handler);
    }

    /**
     * Remove an AnalogValueChangedEventHandler
     *
     * @param handler the AnalogValueChangedEventHandler to remove
     */
    public void removeAnalogValueChangedEventHandler(AnalogValueChangedEventHandler handler) {
        analogValueListeners.remove(handler);
    }

    /**
     * Add an AnalogVoltageChangedEventHandler
     *
     * @param handler the AnalogVoltageChangedEventHandler to add
     */
    public void addAnalogVoltageChangedEventHandler(AnalogVoltageChangedEventHandler handler) {
        analogVoltageListeners.add(handler);
    }

    /**
     * Remove an AnalogVoltageChangedEventHandler
     *
     * @param handler the AnalogVoltageChangedEventHandler to remove
     */
    public void removeAnalogVoltageChangedEventHandler(AnalogVoltageChangedEventHandler handler) {
        analogVoltageListeners.remove(handler);
    }

    /**
     * Gets the threshold the ADC value must change by for the event to fire
     *
     * @return the ADC threshold value
     */
    public int getAdcValueChangedThreshold() {
        return adcValueChangedThreshold;
    }

    /**
     * Sets the threshold the ADC value must change by for the event to fire
     *
     * @param adcValueChangedThreshold the ADC threshold value
     */
    public void setAdcValueChangedThreshold(int adcValueChangedThreshold) {
        this.adcValueChangedThreshold = adcValueChangedThreshold;
    }

    /**
     * Gets the threshold the analog value must change by for the event to fire
     *
     * @return the analog threshold value
     */
    public double getAnalogValueChangedThreshold() {
        return analogValueChangedThreshold;
    }

    /**
     * Sets the threshold the ADC value must change by for the event to fire
     *
     * @param analogValueChangedThreshold the ADC threshold value
     */
    public void setAnalogValueChangedThreshold(double analogValueChangedThreshold) {
        this.analogValueChangedThreshold = analogValueChangedThreshold;
    }

    /**
     * Gets the threshold the analog voltage must change by for the event to fire
     *
     * @return the analog voltage threshold value
     */
    public double getAnalogVoltageChangedThreshold() {
        return analogVoltageChangedThreshold;
    }

    /**
     * Sets the threshold the analog voltage must change by for the event to fire
     *
     * @param analogVoltageChangedThreshold the analog voltage threshold value
     */
    public void setAnalogVoltageChangedThreshold(double analogVoltageChangedThreshold) {
        this.analogVoltageChangedThreshold = analogVoltageChangedThreshold;
    }

    void RaiseAnalogInChanged() {
        if (abs(prevAdcValue - adcValue) > adcValueChangedThreshold) {
            AdcValueChangedEventArgs eventArgs = new AdcValueChangedEventArgs(adcValue, prevAdcValue);
            for (AdcValueChangedEventHandler handler : adcValueListeners)
                handler.adcValueChanged(this, eventArgs);

            prevAdcValue = adcValue;
        }

        if (abs(prevAnalogVoltage - getAnalogVoltage()) > analogVoltageChangedThreshold) {
            AnalogVoltageChangedEventArgs eventArgs = new AnalogVoltageChangedEventArgs(getAnalogVoltage(), prevAnalogVoltage);
            for (AnalogVoltageChangedEventHandler handler : analogVoltageListeners)
                handler.analogVoltageChanged(this, eventArgs);

            prevAnalogVoltage = getAnalogVoltage();
        }

        if (abs(prevAnalogValue - getAnalogValue()) > analogValueChangedThreshold) {
            AnalogValueChangedEventArgs eventArgs = new AnalogValueChangedEventArgs(getAnalogValue(), prevAnalogValue);
            for (AnalogValueChangedEventHandler handler : analogValueListeners)
                handler.analogValueChanged(this, eventArgs);

            prevAnalogValue = getAnalogValue();
        }
    }

    /**
     * Get the ADC value, 0-4091, of the pin
     *
     * @return the ADC value of the pin
     */
    public int getAdcValue() {
        return adcValue;
    }

    /**
     * Gets the analog voltage (0-3.3) of this pin
     *
     * @return the analog voltage of the pin
     */
    public double getAnalogVoltage() {
        return adcValue * (getReferenceLevelVoltage() / 4092.0);
    }

    /**
     * Gets the analog value (from 0-1) of this pin
     *
     * @return the last-read analog value of the pin
     */
    public double getAnalogValue() {
        return adcValue / 4092.0;
    }

    /**
     * Gets the AdcReferenceLevel of this pin
     *
     * @return the ADC reference level of the pin
     */
    public AdcReferenceLevel getReferenceLevel() {
        return referenceLevel;
    }

    /**
     * Set the AdcReferenceLevel of this pin
     *
     * @param referenceLevel the ADC reference level to use
     */
    public void setReferenceLevel(AdcReferenceLevel referenceLevel) {
        this.referenceLevel = referenceLevel;
    }

    double getReferenceLevelVoltage() {
        switch (referenceLevel) {
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