package io.treehopper.libraries;

import io.treehopper.SMBusDevice;

public class SMBusRegisterManagerAdapter implements IRegisterManagerAdapter {

    SMBusDevice device;

    public SMBusRegisterManagerAdapter(SMBusDevice device)
    {
        this.device = device;
    }

    @Override
    public byte[] read(int address, int width) {
        return device.readBufferData((byte)address, width);
    }

    @Override
    public void write(int address, byte[] data) {
        device.writeBufferData((byte)address, data);
    }
}
