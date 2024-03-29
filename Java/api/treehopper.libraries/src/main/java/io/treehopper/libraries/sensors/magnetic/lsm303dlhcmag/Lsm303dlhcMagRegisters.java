/// This file was auto-generated by RegisterGenerator. Any changes to it will be overwritten!
package io.treehopper.libraries.sensors.magnetic.lsm303dlhcmag;

import io.treehopper.libraries.IRegisterManagerAdapter;
import io.treehopper.libraries.RegisterManager;
import io.treehopper.libraries.Register;
import java.util.Arrays;

class Lsm303dlhcMagRegisters extends RegisterManager
{
    Lsm303dlhcMagRegisters(IRegisterManagerAdapter adapter)
    {
        super(adapter);
        tempOut = new TempOutRegister(this);
        _registers.add(tempOut);
        sr = new SrRegister(this);
        _registers.add(sr);
        cra = new CraRegister(this);
        _registers.add(cra);
        crb = new CrbRegister(this);
        _registers.add(crb);
        mr = new MrRegister(this);
        _registers.add(mr);
        outX = new OutXRegister(this);
        _registers.add(outX);
        outY = new OutYRegister(this);
        _registers.add(outY);
        outZ = new OutZRegister(this);
        _registers.add(outZ);
    }

    TempOutRegister tempOut;
    SrRegister sr;
    CraRegister cra;
    CrbRegister crb;
    MrRegister mr;
    OutXRegister outX;
    OutYRegister outY;
    OutZRegister outZ;

    class TempOutRegister extends Register
    {
        TempOutRegister(RegisterManager regManager) { super(regManager, 0x05, 2, false); }

        int value;


        public TempOutRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((value & 0xFFF) << 4); }
        public void setValue(long _value)
        {
            value = (int)(((_value >> 4) & 0xFFF) << (32 - 12)) >> (32 - 12);
        }
    }
    class SrRegister extends Register
    {
        SrRegister(RegisterManager regManager) { super(regManager, 0x09, 1, false); }

        int drdy;
        int registerLock;


        public SrRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((drdy & 0x1) << 0) | ((registerLock & 0x1) << 1); }
        public void setValue(long _value)
        {
            drdy = (int)((_value >> 0) & 0x1);
            registerLock = (int)((_value >> 1) & 0x1);
        }
    }
    class CraRegister extends Register
    {
        CraRegister(RegisterManager regManager) { super(regManager, 0x80, 1, false); }

        int magDataRate;
        int tempEnable;

                public MagDataRates getMagDataRate() { for (MagDataRates b : MagDataRates.values()) { if(b.getVal() == magDataRate) return b; } return MagDataRates.values()[0]; }
                public void setMagDataRate(MagDataRates enumVal) { magDataRate = enumVal.getVal(); }

        public CraRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((magDataRate & 0x7) << 2) | ((tempEnable & 0x1) << 7); }
        public void setValue(long _value)
        {
            magDataRate = (int)((_value >> 2) & 0x7);
            tempEnable = (int)((_value >> 7) & 0x1);
        }
    }
    class CrbRegister extends Register
    {
        CrbRegister(RegisterManager regManager) { super(regManager, 0x81, 1, false); }

        int gainConfiguration;

                public GainConfigurations getGainConfiguration() { for (GainConfigurations b : GainConfigurations.values()) { if(b.getVal() == gainConfiguration) return b; } return GainConfigurations.values()[0]; }
                public void setGainConfiguration(GainConfigurations enumVal) { gainConfiguration = enumVal.getVal(); }

        public CrbRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((gainConfiguration & 0x7) << 5); }
        public void setValue(long _value)
        {
            gainConfiguration = (int)((_value >> 5) & 0x7);
        }
    }
    class MrRegister extends Register
    {
        MrRegister(RegisterManager regManager) { super(regManager, 0x82, 1, false); }

        int magSensorMode;

                public MagSensorModes getMagSensorMode() { for (MagSensorModes b : MagSensorModes.values()) { if(b.getVal() == magSensorMode) return b; } return MagSensorModes.values()[0]; }
                public void setMagSensorMode(MagSensorModes enumVal) { magSensorMode = enumVal.getVal(); }

        public MrRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((magSensorMode & 0x3) << 0); }
        public void setValue(long _value)
        {
            magSensorMode = (int)((_value >> 0) & 0x3);
        }
    }
    class OutXRegister extends Register
    {
        OutXRegister(RegisterManager regManager) { super(regManager, 0x83, 2, false); }

        int value;


        public OutXRegister read()
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
    class OutYRegister extends Register
    {
        OutYRegister(RegisterManager regManager) { super(regManager, 0x85, 2, false); }

        int value;


        public OutYRegister read()
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
    class OutZRegister extends Register
    {
        OutZRegister(RegisterManager regManager) { super(regManager, 0x87, 2, false); }

        int value;


        public OutZRegister read()
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
}
