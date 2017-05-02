package io.treehopper.libraries;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;

/**
 * Created by JayLocal on 5/2/2017.
 */
public abstract class Register {
    protected RegisterManager manager;
    public boolean isLittleEndian;
    public int address;

    public Register(RegisterManager regManager, int address, int width, boolean isBigEndian)
    {
        manager = regManager;
        this.width = width;
        isLittleEndian = !isBigEndian;
        this.address = address;
    }

    public void write()
    {
        manager.write(this);
    }

    public int width;
    public abstract long getValue();
    public abstract void setValue(long value);

    byte[] getBytes()
    {
        byte[] bytes = new byte[width];

        if(isLittleEndian)
        {
            for (int i = 0; i < bytes.length; i++)
                bytes[i] = (byte)((getValue() >> (8 * i)) & 0xFF);
        } else {
            for (int i = bytes.length-1; i >= 0; i--)
                bytes[i] = (byte)((getValue() >> (8 * i)) & 0xFF);
        }

        return bytes;
    }

    void setBytes(byte[] bytes)
    {
        long regVal = 0;
        if(isLittleEndian)
        {
            for (int i = 0; i < bytes.length; i++)
                regVal |= (bytes[i] & 0xff) << (i * 8);
        } else {
            for (int i = 0; i < bytes.length; i++)
                regVal |= (bytes[i] & 0xff) << ((bytes.length - i - 1) * 8);
        }

        setValue(regVal);
    }
}
