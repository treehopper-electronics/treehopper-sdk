package io.treehopper.libraries.displays;

import io.treehopper.*;

/**
 * Created by jay on 12/5/2016.
 */

public class Tm1650 extends LedDriver {
    private I2c i2c;
    private static final byte ControlBase = 0x24;
    private static final byte DisplayBase = 0x34;
    private byte[] oldValues = new byte[4];
    private byte[] newValues = new byte[4];

    public boolean isEnabled() {
        return enable;
    }

    public void setEnabled(boolean enable) {
        if(this.enable == enable) return;
        this.enable = enable;
        sendControlUpdate();
    }

    private boolean enable;

    public Tm1650(I2c i2c)
    {
        super(32, false, false);
        this.i2c = i2c;
        i2c.setEnabled(true);
        this.enable = true;
        this.clear();
    }

    private void sendControlUpdate()
    {
        byte controlByte = (byte)(enable ? 0x01 : 0x00);
        for (int i = 0; i < 4; i++)
            sendControl(controlByte, i);
    }

    private void sendControl(byte data, int digit)
    {
        i2c.sendReceive((byte)(ControlBase + digit), new byte[] { data }, 0);
    }

    private void sendDisplay(byte data, int digit)
    {
        i2c.sendReceive((byte)(DisplayBase + digit), new byte[] { data }, 0);
    }

    @Override
    public boolean hasGlobalBrightnessControl() {
        return true;
    }

    @Override
    public boolean hasIndividualBrightnessControl() {
        return false;
    }

    @Override
    public double getBrightness() {
        return 0;
    }

    @Override
    public void setBrightness(double brightness) {

    }

    @Override
    public void flush(boolean force)
    {
        for(int i=0;i<4;i++)
        {
            if (oldValues[i] != newValues[i] || force)
            {
                sendDisplay(newValues[i], i);
                oldValues[i] = newValues[i];
            }
        }
    }

    public void ledBrightnessChanged(Led led)
    {

    }

    public void ledStateChanged(Led led)
    {
        int digit = led.getChannel() / 8;
        int segment = led.getChannel() % 8;
        boolean value = led.getState();

        // set or clear the appropriate bit
        if(led.getState())
            newValues[digit] |= (byte)(1 << segment);
        else
            newValues[digit] &= (byte)~(1 << segment);

        if (isAutoFlushEnabled())
            flush(false);
    }

}
