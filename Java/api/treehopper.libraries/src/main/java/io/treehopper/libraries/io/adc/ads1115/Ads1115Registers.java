/// This file was auto-generated by RegisterGenerator. Any changes to it will be overwritten!
package io.treehopper.libraries.io.adc.ads1115;

import io.treehopper.libraries.IRegisterManagerAdapter;
import io.treehopper.libraries.RegisterManager;
import io.treehopper.libraries.Register;
import java.util.Arrays;

class Ads1115Registers extends RegisterManager
{
    Ads1115Registers(IRegisterManagerAdapter adapter)
    {
        super(adapter);
        conversion = new ConversionRegister(this);
        _registers.add(conversion);
        config = new ConfigRegister(this);
        _registers.add(config);
        lowThreshold = new LowThresholdRegister(this);
        _registers.add(lowThreshold);
        highThreshold = new HighThresholdRegister(this);
        _registers.add(highThreshold);
    }

    ConversionRegister conversion;
    ConfigRegister config;
    LowThresholdRegister lowThreshold;
    HighThresholdRegister highThreshold;

    class ConversionRegister extends Register
    {
        ConversionRegister(RegisterManager regManager) { super(regManager, 0x00, 2, true); }

        int value;


        public ConversionRegister read()
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
    class ConfigRegister extends Register
    {
        ConfigRegister(RegisterManager regManager) { super(regManager, 0x01, 2, false); }

        int comparatorQueue;
        int latchingComparator;
        int comparatorPolarity;
        int comparatorMode;
        int dataRate;
        int operatingMode;
        int pga;
        int mux;
        int operationalStatus;

                public ComparatorQueues getComparatorQueue() { for (ComparatorQueues b : ComparatorQueues.values()) { if(b.getVal() == comparatorQueue) return b; } return ComparatorQueues.values()[0]; }
                public void setComparatorQueue(ComparatorQueues enumVal) { comparatorQueue = enumVal.getVal(); }
                public DataRates getDataRate() { for (DataRates b : DataRates.values()) { if(b.getVal() == dataRate) return b; } return DataRates.values()[0]; }
                public void setDataRate(DataRates enumVal) { dataRate = enumVal.getVal(); }
                public Pgas getPga() { for (Pgas b : Pgas.values()) { if(b.getVal() == pga) return b; } return Pgas.values()[0]; }
                public void setPga(Pgas enumVal) { pga = enumVal.getVal(); }
                public Muxes getMux() { for (Muxes b : Muxes.values()) { if(b.getVal() == mux) return b; } return Muxes.values()[0]; }
                public void setMux(Muxes enumVal) { mux = enumVal.getVal(); }

        public ConfigRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((comparatorQueue & 0x3) << 0) | ((latchingComparator & 0x1) << 2) | ((comparatorPolarity & 0x1) << 3) | ((comparatorMode & 0x1) << 4) | ((dataRate & 0x7) << 5) | ((operatingMode & 0x1) << 8) | ((pga & 0x7) << 9) | ((mux & 0x7) << 12) | ((operationalStatus & 0x1) << 15); }
        public void setValue(long _value)
        {
            comparatorQueue = (int)((_value >> 0) & 0x3);
            latchingComparator = (int)((_value >> 2) & 0x1);
            comparatorPolarity = (int)((_value >> 3) & 0x1);
            comparatorMode = (int)((_value >> 4) & 0x1);
            dataRate = (int)((_value >> 5) & 0x7);
            operatingMode = (int)((_value >> 8) & 0x1);
            pga = (int)((_value >> 9) & 0x7);
            mux = (int)((_value >> 12) & 0x7);
            operationalStatus = (int)((_value >> 15) & 0x1);
        }
    }
    class LowThresholdRegister extends Register
    {
        LowThresholdRegister(RegisterManager regManager) { super(regManager, 0x02, 2, true); }

        int value;


        public LowThresholdRegister read()
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
    class HighThresholdRegister extends Register
    {
        HighThresholdRegister(RegisterManager regManager) { super(regManager, 0x03, 2, true); }

        int value;


        public HighThresholdRegister read()
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
