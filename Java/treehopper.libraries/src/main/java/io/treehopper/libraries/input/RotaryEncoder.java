package io.treehopper.libraries.input;

import java.util.ArrayList;

import io.treehopper.events.DigitalInValueChangedEventArgs;
import io.treehopper.events.DigitalInValueChangedEventHandler;
import io.treehopper.interfaces.DigitalInPin;

/**
 * Created by jay on 2/17/2017.
 */

public class RotaryEncoder {
    private final int stepsPerTick;
    private final DigitalInPin a;
    private final DigitalInPin b;
    private long position = 0;
    long oldPosition;
    private ArrayList<PositionChangedEventHandler> positionChangedEventHandlers = new ArrayList<>();

    public RotaryEncoder(DigitalInPin channelA, DigitalInPin channelB, int stepsPerTick)
    {
        this.stepsPerTick = stepsPerTick;
        a = channelA;
        b = channelB;

        a.makeDigitalIn();
        b.makeDigitalIn();

        setPosition(0);

        a.addDigitalInValueChangedEventHandler(new DigitalInValueChangedEventHandler() {
            @Override
            public void DigitalValueChanged(Object sender, DigitalInValueChangedEventArgs e) {
                if(b.getDigitalValue() && a.getDigitalValue())
                {
                    position++;
                }
                else if(b.getDigitalValue() && !a.getDigitalValue())
                {
                    position--;
                }
                else if(!b.getDigitalValue() && a.getDigitalValue())
                {
                    position--;
                }
                else if (!b.getDigitalValue() && !a.getDigitalValue())
                {
                    position++;
                }

                updatePosition();
            }
        });

        b.addDigitalInValueChangedEventHandler(new DigitalInValueChangedEventHandler() {
            @Override
            public void DigitalValueChanged(Object sender, DigitalInValueChangedEventArgs e) {
                if(b.getDigitalValue() && a.getDigitalValue())
                {
                    position--;
                }
                else if(b.getDigitalValue() && !a.getDigitalValue())
                {
                    position++;
                }
                else if(!b.getDigitalValue() && a.getDigitalValue())
                {
                    position++;
                }
                else if (!b.getDigitalValue() && !a.getDigitalValue())
                {
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

    private void updatePosition()
    {
        if (position % stepsPerTick == 0)
        {
            if (getPosition() == oldPosition) return;

            PositionChangedEventArgs args = new PositionChangedEventArgs();
            args.newPosition = getPosition();

            for(PositionChangedEventHandler handler : positionChangedEventHandlers)
            {
                handler.PositionChanged(this, args);
            }

            oldPosition = getPosition();
        }
    }

    public void addPositionChangedEventHandler(PositionChangedEventHandler handler)
    {
        this.positionChangedEventHandlers.add(handler);
    }

    public void removePositionChangedEventHandler(PositionChangedEventHandler handler)
    {
        this.positionChangedEventHandlers.remove(handler);
    }

    public interface PositionChangedEventHandler {
        void PositionChanged(Object sender, PositionChangedEventArgs e);
    }

    public class PositionChangedEventArgs
    {
        public long newPosition;
    }
}
