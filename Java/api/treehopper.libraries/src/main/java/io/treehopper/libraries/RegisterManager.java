package io.treehopper.libraries;

import io.treehopper.SMBusDevice;

import java.util.ArrayList;
import java.util.Arrays;

/**
 * Created by JayLocal on 5/2/2017.
 */
public class RegisterManager {
    protected ArrayList<Register> _registers = new ArrayList<>();
    protected SMBusDevice _dev;

    public RegisterManager(SMBusDevice dev)
    {
        _dev = dev;
    }

    public void read(Register register)
    {
        register.setBytes(_dev.readBufferData((byte) register.address, register.width));
    }

    public void readRange(Register start, Register end)
    {
        int count = (end.address + end.width) - start.address;
        byte[] bytes = _dev.readBufferData((byte) start.address, count);
        int i = 0;

        for(Register reg : _registers)
        {
            if(reg.address < start.address || reg.address > end.address)
                continue;

            reg.setBytes(Arrays.copyOfRange(bytes, i, reg.width + i));
            i += reg.width;
        }
    }

    public void write(Register register)
    {
        _dev.writeBufferData((byte) register.address, register.getBytes());
    }

    public void writeRange(Register start, Register end)
    {
        ArrayList<Byte> bytes = new ArrayList<>();
        for(Register reg : _registers)
        {
            if(reg.address < start.address || reg.address > end.address)
                continue;

            byte[] readBytes = reg.getBytes();
            for(int i = 0; i< readBytes.length;i++)
                bytes.add(readBytes[i]);
        }

        byte[] bytesToWrite = new byte[bytes.size()];
        for(int i = 0; i < bytes.size();i++)
        {
            bytesToWrite[i] = bytes.get(i);
        }

        _dev.writeBufferData((byte) start.address, bytesToWrite);
    }
}
