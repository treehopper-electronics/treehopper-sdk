package io.treehopper.libraries.input;

import io.treehopper.events.DigitalInValueChangedEventArgs;
import io.treehopper.events.DigitalInValueChangedEventHandler;
import io.treehopper.interfaces.DigitalIn;

import java.util.ArrayList;

/**
 * Quadrature rotary encoder
 */
public class RotaryEncoder {
    private final int stepsPerTick;
    private final DigitalIn a;
    private final DigitalIn b;
    long oldPosition;
    private long position = 0;
    private ArrayList<PositionChangedEventHandler> positionChangedEventHandlers = new ArrayList<>();

    public RotaryEncoder(DigitalIn channelA, DigitalIn channelB, int stepsPerTick) {
        this.stepsPerTick = stepsPerTick;
        a = channelA;
        b = channelB;

        a.makeDigitalIn();
        b.makeDigitalIn();

        setPosition(0);

        a.addDigitalInValueChangedEventHandler(new DigitalInValueChangedEventHandler() {
            @Override
            public void DigitalValueChanged(Object sender, DigitalInValueChangedEventArgs e) {
                if (b.getDigitalValue() && a.getDigitalValue()) {
                    position++;
                } else if (b.getDigitalValue() && !a.getDigitalValue()) {
                    position--;
                } else if (!b.getDigitalValue() && a.getDigitalValue()) {
                    position--;
                } else if (!b.getDigitalValue() && !a.getDigitalValue()) {
                    position++;
                }

                updatePosition();
            }
        });

        b.addDigitalInValueChangedEventHandler(new DigitalInValueChangedEventHandler() {
            @Override
            public void DigitalValueChanged(Object sender, DigitalInValueChangedEventArgs e) {
                if (b.getDigitalValue() && a.getDigitalValue()) {
                    position--;
                } else if (b.getDigitalValue() && !a.getDigitalValue()) {
                    position++;
                } else if (!b.getDigitalValue() && a.getDigitalValue()) {
                    position++;
                } else if (!b.getDigitalValue() && !a.getDigitalValue()) {
                    position--;
                }

                updatePosition();
            }
        });
    }

    public long getPosition() {
        return position / stepsPerTick;
    }

    public void setPosition(long position) {
        this.position = position * stepsPerTick;
    }

    private void updatePosition() {
        if (position % stepsPerTick == 0) {
            if (getPosition() == oldPosition) return;

            PositionChangedEventArgs args = new PositionChangedEventArgs();
            args.newPosition = getPosition();

            for (PositionChangedEventHandler handler : positionChangedEventHandlers) {
                handler.PositionChanged(this, args);
            }

            oldPosition = getPosition();
        }
    }

    public void addPositionChangedEventHandler(PositionChangedEventHandler handler) {
        this.positionChangedEventHandlers.add(handler);
    }

    public void removePositionChangedEventHandler(PositionChangedEventHandler handler) {
        this.positionChangedEventHandlers.remove(handler);
    }

    /**
     * Rotary encoder position changed event handler
     */
    public interface PositionChangedEventHandler {
        void PositionChanged(Object sender, PositionChangedEventArgs e);
    }

    /**
     * Rotary encoder position changed EventArgs
     */
    public class PositionChangedEventArgs {
        public long newPosition;
    }
}
