package io.treehopper;

import io.treehopper.enums.PinMode;

import java.util.*;

/**
 * Internal SoftPWM manager
 */
public class SoftPwmManager {

    private final HashMap<Integer, SoftPwmConfig> _pins;
    private final TreehopperUsb _board;
    private double _resolution = 0.25; // 0.25 microseconds / tick
    Object lock = new Object();

    public SoftPwmManager(TreehopperUsb board) {
        this._board = board;
        this._pins = new HashMap<Integer, SoftPwmConfig>();
    }

    public void stop() {
        for(SoftPwmConfig config : _pins.values())
        {
            config.Pin.setMode(PinMode.DigitalInput);
        }

        _pins.clear();
        updateConfig();
    }

    private void updateConfig() {
        if(_pins.size() > 0)
        {
            for(SoftPwmConfig entry : _pins.values())
            {
                // for pins that use pulse width, calculate value based on resolution
                if(entry.UsePulseWidth)
                {
                    entry.Ticks = (int)(entry.PulseWidthUs / _resolution);

                    // just in case the user wants to retrieve duty cycle, update its value, too
                    entry.DutyCycle = entry.Ticks / 65535d;
                }
                else
                {
                    // for pins that use duty-cycle, calculate based on period count
                    entry.Ticks = (int)Math.round(entry.DutyCycle * 65535);

                    // just in case the user wants to retrieve pulse width, update its value too
                    entry.PulseWidthUs = (int) (entry.Ticks * _resolution);
                }
            }


            Comparator<SoftPwmConfig> comparator = new Comparator<SoftPwmConfig>() {
                @Override
                public int compare(SoftPwmConfig left, SoftPwmConfig right) {
                    return left.Ticks - right.Ticks; // use your logic
                }
            };

            // now the fun part; let's figure out the delta delays between each pin
            ArrayList<SoftPwmConfig> list = new ArrayList<SoftPwmConfig>(_pins.values());
            Collections.sort(list, comparator);

            int count = list.size() + 1;
            byte[] config = new byte[2 + 3 * count]; // { , (byte)pins.Count, timerVal };
            config[0] = (byte) DeviceCommands.SoftPwmConfig.ordinal();
            config[1] = (byte) count;
            if (count > 1)
            {
                int i = 2;
                int time = 0;

                for (int j = 0; j < count; j++)
                {
                    int ticks;

                    if (j < list.size())
                        ticks = list.get(j).Ticks - time;
                    else
                        ticks = 65535 - time;

                    int tmrVal = 65535 - ticks;
                    if (j == 0)
                        config[i++] = 0;
                    else
                        config[i++] = (byte) list.get(j - 1).Pin.getPinNumber();

                    config[i++] = (byte) ((tmrVal >> 8) & 0xff);
                    config[i++] = (byte) (tmrVal & 0xff);
                    time += ticks;
                }
            }
            else
            {
                config[1] = 0;
            }

            _board.sendPeripheralConfigPacket(config);
        }
        else
        {
            // disable SoftPWM
            _board.sendPeripheralConfigPacket(new byte[] {(byte) DeviceCommands.SoftPwmConfig.ordinal(), 0});
        }
    }

    @Override
    protected void finalize() throws Throwable {
        super.finalize();
        stop();
    }

    public double getDutyCycle(Pin pin) {
        if(!_pins.containsKey(pin.getPinNumber())) return 0;

        return _pins.get(pin.getPinNumber()).DutyCycle;
    }

    public void setDutyCycle(Pin pin, double value) {
        if(!_pins.containsKey(pin.getPinNumber())) return;

        synchronized (lock)
        {
            _pins.get(pin.getPinNumber()).DutyCycle = value;
            _pins.get(pin.getPinNumber()).UsePulseWidth = false;
            updateConfig();
        }
    }

    public double getPulseWidth(Pin pin) {
        if(!_pins.containsKey(pin.getPinNumber())) return 0;

        return _pins.get(pin.getPinNumber()).PulseWidthUs;
    }

    public void setPulseWidth(Pin pin, double pulseWidth) {
        if(!_pins.containsKey(pin.getPinNumber())) return;

        synchronized (lock)
        {
            _pins.get(pin.getPinNumber()).PulseWidthUs = pulseWidth;
            _pins.get(pin.getPinNumber()).UsePulseWidth = true;
            updateConfig();
        }
    }

    public void StartPin(Pin pin) {
        if (_pins.containsKey(pin.getPinNumber()))
            return;

        _pins.put(pin.getPinNumber(), new SoftPwmConfig(pin, 0, true));
        updateConfig();
    }

    public void StopPin(Pin pin) {
        _pins.remove(pin.getPinNumber());

        updateConfig();

    }
}
