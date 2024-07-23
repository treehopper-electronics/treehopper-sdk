/// This file was auto-generated by RegisterGenerator. Any changes to it will be overwritten!
package io.treehopper.libraries.sensors.magnetic.ak8975;

import io.treehopper.libraries.IRegisterManagerAdapter;
import io.treehopper.libraries.RegisterManager;
import io.treehopper.libraries.Register;
import java.util.Arrays;

class Ak8975Registers extends RegisterManager
{
    Ak8975Registers(IRegisterManagerAdapter adapter)
    {
        super(adapter);
        wia = new WiaRegister(this);
        _registers.add(wia);
        info = new InfoRegister(this);
        _registers.add(info);
        status1 = new Status1Register(this);
        _registers.add(status1);
        hx = new HxRegister(this);
        _registers.add(hx);
        hy = new HyRegister(this);
        _registers.add(hy);
        hz = new HzRegister(this);
        _registers.add(hz);
        status2 = new Status2Register(this);
        _registers.add(status2);
        control = new ControlRegister(this);
        _registers.add(control);
        sensitivityX = new SensitivityXRegister(this);
        _registers.add(sensitivityX);
        sensitivityY = new SensitivityYRegister(this);
        _registers.add(sensitivityY);
        sensitivityZ = new SensitivityZRegister(this);
        _registers.add(sensitivityZ);
    }

    WiaRegister wia;
    InfoRegister info;
    Status1Register status1;
    HxRegister hx;
    HyRegister hy;
    HzRegister hz;
    Status2Register status2;
    ControlRegister control;
    SensitivityXRegister sensitivityX;
    SensitivityYRegister sensitivityY;
    SensitivityZRegister sensitivityZ;

    class WiaRegister extends Register
    {
        WiaRegister(RegisterManager regManager) { super(regManager, 0x00, 1, false); }

        int value;


        public WiaRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((value & 0xFF) << 0); }
        public void setValue(long _value)
        {
            value = (int)((_value >> 0) & 0xFF);
        }
    }
    class InfoRegister extends Register
    {
        InfoRegister(RegisterManager regManager) { super(regManager, 0x01, 1, false); }

        int value;


        public InfoRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((value & 0xFF) << 0); }
        public void setValue(long _value)
        {
            value = (int)((_value >> 0) & 0xFF);
        }
    }
    class Status1Register extends Register
    {
        Status1Register(RegisterManager regManager) { super(regManager, 0x02, 1, false); }

        int drdy;


        public Status1Register read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((drdy & 0x1) << 0); }
        public void setValue(long _value)
        {
            drdy = (int)((_value >> 0) & 0x1);
        }
    }
    class HxRegister extends Register
    {
        HxRegister(RegisterManager regManager) { super(regManager, 0x03, 2, false); }

        int value;


        public HxRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((value & 0xFFFF) << 0); }
        public void setValue(long _value)
        {
            value = (int)(((_value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
        }
    }
    class HyRegister extends Register
    {
        HyRegister(RegisterManager regManager) { super(regManager, 0x05, 2, false); }

        int value;


        public HyRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((value & 0xFFFF) << 0); }
        public void setValue(long _value)
        {
            value = (int)(((_value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
        }
    }
    class HzRegister extends Register
    {
        HzRegister(RegisterManager regManager) { super(regManager, 0x07, 2, false); }

        int value;


        public HzRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((value & 0xFFFF) << 0); }
        public void setValue(long _value)
        {
            value = (int)(((_value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
        }
    }
    class Status2Register extends Register
    {
        Status2Register(RegisterManager regManager) { super(regManager, 0x09, 1, false); }

        int derr;
        int hofl;


        public Status2Register read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((derr & 0x1) << 2) | ((hofl & 0x1) << 3); }
        public void setValue(long _value)
        {
            derr = (int)((_value >> 2) & 0x1);
            hofl = (int)((_value >> 3) & 0x1);
        }
    }
    class ControlRegister extends Register
    {
        ControlRegister(RegisterManager regManager) { super(regManager, 0x0a, 1, false); }

        int mode;


        public ControlRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((mode & 0xF) << 0); }
        public void setValue(long _value)
        {
            mode = (int)((_value >> 0) & 0xF);
        }
    }
    class SensitivityXRegister extends Register
    {
        SensitivityXRegister(RegisterManager regManager) { super(regManager, 0x10, 1, false); }

        int value;


        public SensitivityXRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((value & 0xFF) << 0); }
        public void setValue(long _value)
        {
            value = (int)((_value >> 0) & 0xFF);
        }
    }
    class SensitivityYRegister extends Register
    {
        SensitivityYRegister(RegisterManager regManager) { super(regManager, 0x11, 1, false); }

        int value;


        public SensitivityYRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((value & 0xFF) << 0); }
        public void setValue(long _value)
        {
            value = (int)((_value >> 0) & 0xFF);
        }
    }
    class SensitivityZRegister extends Register
    {
        SensitivityZRegister(RegisterManager regManager) { super(regManager, 0x12, 1, false); }

        int value;


        public SensitivityZRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((value & 0xFF) << 0); }
        public void setValue(long _value)
        {
            value = (int)((_value >> 0) & 0xFF);
        }
    }
}
