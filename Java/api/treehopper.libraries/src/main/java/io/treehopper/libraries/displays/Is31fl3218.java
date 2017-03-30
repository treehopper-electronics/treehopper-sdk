package io.treehopper.libraries.displays;

import io.treehopper.SMBusDevice;
import io.treehopper.interfaces.I2c;

/**
 * ISSI IS31F13218 I2c 18-channel 16-bit PWM constant-current LED driver
 */
public class Is31fl3218 extends LedDriver {
	
    private I2c i2c;
	private boolean shutdown;
    private byte[] currentValues;
    private boolean[] currentStates;
    private SMBusDevice dev;
    
	
	public Is31fl3218(I2c i2c, int rateKhz) {
		super(18, true, true);
		
		this.i2c = i2c;
		currentValues = new byte[18];
		currentStates = new boolean[18];
		
		byte address = 0x54;
		dev = new SMBusDevice((byte)address, i2c, rateKhz);
		byte shutdown = 0x00;
		byte data = 0x01;
		dev.writeByteData((byte)shutdown, (byte)data);
	}

	@Override
	public void flush(boolean force) {
        byte[] stateBytes = new byte[3];
        for (int i = 0; i<currentStates.length;i++)
        {
            int theBit = i % 6;
            int theByte = i / 6;
            if (currentStates[i])
                stateBytes[theByte] |= (byte)(1 << theBit);
            else
                stateBytes[theByte] &= (byte)~(1 << theBit);
        }

		byte[] dataToWrite = new byte[currentValues.length + stateBytes.length + 1];
        System.arraycopy(currentValues, 0, dataToWrite, 0, currentValues.length);
        System.arraycopy(stateBytes, 0, dataToWrite, currentValues.length, stateBytes.length);
        dev.writeBufferData((byte)0x01, dataToWrite);
		
	}

	@Override
	public boolean hasGlobalBrightnessControl() {
		return true;
	}

	@Override
	public boolean hasIndividualBrightnessControl() {
		return true;
	}

	@Override
	protected void _setBrightness(double brightness) {
		boolean autoflushSave = isAutoFlushEnabled();
		setAutoFlushEnabled(false);
		for(Led led : this.getLeds())
			led.setBrightness(brightness);

		if(autoflushSave)
			flush(false);

		setAutoFlushEnabled(autoflushSave);
	}

	@Override
	public void ledStateChanged(Led led) {
		currentStates[led.getChannel()] = led.getState();
		if(isAutoFlushEnabled()){
			flush(false);
		}
	}

	@Override
	public void ledBrightnessChanged(Led led) {
		currentValues[led.getChannel()] = (byte) Math.round(led.getBrightness()*255);
		if(isAutoFlushEnabled()){
			flush(false);
		}
	}



	

}
