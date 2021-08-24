package io.treehopper.libraries.sensors.optical.isl29125;

import io.treehopper.SMBusDevice;
import io.treehopper.Utility;
import io.treehopper.events.AdcValueChangedEventHandler;
import io.treehopper.events.DigitalInValueChangedEventArgs;
import io.treehopper.events.DigitalInValueChangedEventHandler;
import io.treehopper.interfaces.DigitalIn;
import io.treehopper.interfaces.I2c;
import io.treehopper.libraries.SMBusRegisterManagerAdapter;
import io.treehopper.libraries.sensors.Pollable;

import java.util.ArrayList;
import java.util.List;

public class Isl29125 extends Pollable {
    private final Isl29125Registers registers;

    public Isl29125(I2c i2c) { this(i2c, null, 100); }

    public Isl29125(I2c i2c, DigitalIn interruptPin) { this(i2c, interruptPin, 100); }

    public Isl29125(I2c i2c, DigitalIn interruptPin, int rate)
    {
        this.registers = new Isl29125Registers(new SMBusRegisterManagerAdapter(new SMBusDevice((byte) 0x44, i2c, rate)));

        // check device ID
        registers.deviceId.read();
        if(registers.deviceId.value != 0x7D)
        {
            Utility.error("No ISL29125 found.", true);
        }

        // reset device
        registers.deviceReset.value = 0x46;
        registers.deviceReset.write();

        registers.config1.setMode(Modes.GreenRedBlue);
        registers.config1.setRange(Ranges.Lux_10000);
        registers.config1.write();

        // optional interrupt setup
        if(interruptPin != null)
        {
            interruptPin.makeDigitalIn();
            interruptPin.addDigitalInValueChangedEventHandler(new DigitalInValueChangedEventHandler() {
                @Override
                public void DigitalValueChanged(Object sender, DigitalInValueChangedEventArgs e) {
                    if(e.newValue == false) // interrupts occur on the falling edge
                    {
                        update();
                        for (InterruptReceivedEventHandler handler : interruptReceivedHandlers)
                            handler.interruptReceived(this, getRed(), getGreen(), getBlue());
                    }
                }
            });

            setInterruptSelection(InterruptSelections.Green);

            // set the default thresholds to 0 so we get an interrupt on every ADC read by default
            setLowThreshold(0);
            setHighThreshold(0);

            // clear the (potentially pending) interrupt
            registers.status.read();
        }
    }

    private List<InterruptReceivedEventHandler> interruptReceivedHandlers = new ArrayList<>();

    public void addInterruptReceivedEventHandler(InterruptReceivedEventHandler handler) { interruptReceivedHandlers.add(handler); }
    public void removeInterruptReceivedEventHandler(InterruptReceivedEventHandler handler) { interruptReceivedHandlers.remove(handler); }

    public Modes getMode()
    {
        return registers.config1.getMode();
    }

    public void setMode(Modes value)
    {
        registers.config1.setMode(value);
        registers.config1.write();
    }

    public SampleDepths getSampleDepth()
    {
        return registers.config1.getSampleDepth();
    }

    public void setSampleDepth(SampleDepths value)
    {
        registers.config1.setSampleDepth(value);
        registers.config1.write();
    }

    public Ranges getRange()
    {
        return registers.config1.getRange();
    }

    public void setRange(Ranges value)
    {
        registers.config1.setRange(value);
        registers.config1.write();
    }

    public int getHighThreshold()
    {
        return registers.highThreshold.value;
    }

    public void setHighThreshold(int value)
    {
        registers.highThreshold.value= value;
        registers.highThreshold.write();
    }

    public int getLowThreshold()
    {
        return registers.lowThreshold.value;
    }

    public void setLowThreshold(int value)
    {
        registers.lowThreshold.value = value;
        registers.config1.write();
    }

    public InterruptSelections getInterruptSelection()
    {
        return registers.config3.getInterruptSelection();
    }

    public void setInterruptSelection(InterruptSelections value)
    {
        registers.config3.setInterruptSelection(value);
        registers.config3.write();
    }

    private double valueToLux(double value)
    {
        if (registers.config1.getRange() == Ranges.Lux_10000)
            value *= 10000;
        else
            value *= 375;

        if (registers.config1.getSampleDepth() == SampleDepths.Bits_12)
            value /= 4095;
        else
            value /= 65535;

        return value;
    }

    double getRed()
    {
        if (isAutoUpdateWhenPropertyRead())
            registers.redData.read();

        return valueToLux(registers.redData.value);
    }

    double getGreen()
    {
        if (isAutoUpdateWhenPropertyRead())
            registers.greenData.read();

        return valueToLux(registers.greenData.value);
    }

    double getBlue()
    {
        if (isAutoUpdateWhenPropertyRead())
            registers.blueData.read();

        return valueToLux(registers.blueData.value);
    }

    @Override
    public void update() {
        registers.readRange(registers.status, registers.blueData);
    }
}
