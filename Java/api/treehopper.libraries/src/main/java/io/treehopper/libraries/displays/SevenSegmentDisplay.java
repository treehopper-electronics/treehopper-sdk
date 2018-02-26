package io.treehopper.libraries.displays;

import java.security.InvalidParameterException;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

/**
 * 7-Segment LED display
 */
public class SevenSegmentDisplay {
    private String text = "";
    private List<SevenSegmentDigit> digits;
    private ArrayList<ILedDriver> drivers = new ArrayList<>();
    private boolean leftAlignt = false;


    public SevenSegmentDisplay(List<Led> Leds, boolean flipDigits) {

        if (Leds.size() % 8 != 0)
            throw new InvalidParameterException("Leds should contain a multiple of 8 segments when using this constructor");

        int numDigits = Leds.size() / 8;

        digits = new ArrayList<SevenSegmentDigit>();

        for (int i = 0; i < numDigits; i++) {
            SevenSegmentDigit digit = new SevenSegmentDigit(Leds.subList(i * 8, i * 8 + 8));
            digits.add(digit);
        }

        if (flipDigits)
            Collections.reverse(digits);

        setupDigits();
    }

    public SevenSegmentDisplay(List<SevenSegmentDigit> Digits) {
        this.digits = Digits;

        setupDigits();
    }

    void setupDigits() {
        for (SevenSegmentDigit digit : digits) {
            // disable per-character auto-flushing for performance
            digit.setAutoFlushEnabled(false);

            for (ILedDriver driver : digit.getDrivers()) {
                if (!drivers.contains(driver))
                    drivers.add(driver);
            }
        }
    }

    public String getText() {
        return text;
    }

    public void setText(String text) {
        if (this.text.equals(text)) return;

        this.text = text;
        printString(text);
    }


    public boolean isLeftAlignt() {
        return leftAlignt;
    }

    public void setLeftAlignt(boolean leftAlignt) {
        this.leftAlignt = leftAlignt;
    }


    private void printString(String text) {
        if (text.length() > digits.size())
            text = text.substring(0, digits.size());

        int k = 0;
        for (int i = 0; i < text.length(); i++) {
            char c = text.charAt(i);

            // print decimal points as part of the previous character
            if (c == '.' && i != 0) continue;
            digits.get(k).setCharacter(c);
            // peak at the next character to look for a decimal point
            if (i + 1 < text.length()) {
                char next = text.charAt(i + 1);
                if (next == '.')
                    digits.get(k).setDecimalPoint(true);
            }
            k++;
        }

        for (ILedDriver driver : drivers) {
            driver.flush(false);
        }
    }
}
