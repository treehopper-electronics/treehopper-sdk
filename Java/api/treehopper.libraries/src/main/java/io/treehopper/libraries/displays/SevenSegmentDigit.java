package io.treehopper.libraries.displays;

import io.treehopper.libraries.IFlushable;

import java.util.ArrayList;
import java.util.List;

/**
 * Single 7-segment LED digit
 */
public class SevenSegmentDigit implements IFlushable {
    static protected final int[] charTable = new int[]{
            //0x00  0x01  0x02  0x03  0x04  0x05  0x06  0x07  0x08  0x09  0x0A  0x0B  0x0C  0x0D  0x0E  0x0F
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // 0x00
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // 0x10
            0x00, 0x82, 0x21, 0x00, 0x00, 0x00, 0x00, 0x02, 0x39, 0x0F, 0x00, 0x00, 0x00, 0x40, 0x80, 0x00, // 0x20
            0x3F, 0x06, 0x5B, 0x4F, 0x66, 0x6D, 0x7D, 0x07, 0x7f, 0x6f, 0x00, 0x00, 0x00, 0x48, 0x00, 0x53, // 0x30
            0x00, 0x77, 0x7C, 0x39, 0x5E, 0x79, 0x71, 0x6F, 0x76, 0x06, 0x1E, 0x00, 0x38, 0x00, 0x54, 0x3F, // 0x40
            0x73, 0x67, 0x50, 0x6D, 0x78, 0x3E, 0x00, 0x00, 0x00, 0x6E, 0x00, 0x39, 0x00, 0x0F, 0x00, 0x08, // 0x50
            0x63, 0x5F, 0x7C, 0x58, 0x5E, 0x7B, 0x71, 0x6F, 0x74, 0x02, 0x1E, 0x00, 0x06, 0x00, 0x54, 0x5C, // 0x60
            0x73, 0x67, 0x50, 0x6D, 0x78, 0x1C, 0x00, 0x00, 0x00, 0x6E, 0x00, 0x39, 0x30, 0x0F, 0x00, 0x00  // 0x70
    };

    private List<Led> leds;
    private boolean autoflush;
    private List<ILedDriver> drivers = new ArrayList<ILedDriver>();
    private char character;
    private boolean decimalPoint;

    public SevenSegmentDigit(List<Led> leds) {
        this.leds = leds;

        for (Led led : leds) {
            led.getDriver().setAutoFlushEnabled(false);

            if (!drivers.contains(led.getDriver())) {
                drivers.add(led.getDriver());
            }
        }

        setCharacter(' ');
        flush(true);

    }

    public char getCharacter() {
        return character;
    }

    public void setCharacter(char value) {
        if (character == value) return;
        character = value;
        int enabledLeds = charTable[character];
        for (int i = 0; i < 7; i++) {
            if (((enabledLeds >> i) & 0x01) == 1)
                leds.get(i).setState(true);
            else
                leds.get(i).setState(false);
        }

        if (isAutoFlushEnabled())
            flush(false);
    }


    public boolean isDecimalPoint() {
        return decimalPoint;
    }

    public void setDecimalPoint(boolean decimalPoint) {
        if (decimalPoint == this.decimalPoint) return;
        this.decimalPoint = decimalPoint;

        leds.get(7).setState(decimalPoint);
    }


    public List<Led> getLeds() {
        return leds;
    }

    public List<ILedDriver> getDrivers() {
        return drivers;
    }


    @Override
    public boolean isAutoFlushEnabled() {
        return autoflush;
    }

    @Override
    public void setAutoFlushEnabled(boolean value) {
        autoflush = value;
    }

    @Override
    public void flush(boolean force) {
        for (ILedDriver driver : drivers) {
            driver.flush(force);
        }
    }
}
