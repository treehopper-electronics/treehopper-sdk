/// This file was auto-generated by RegisterGenerator. Any changes to it will be overwritten!
package io.treehopper.libraries.sensors.optical.vcnl4010;

import io.treehopper.libraries.IRegisterManagerAdapter;
import io.treehopper.libraries.RegisterManager;
import io.treehopper.libraries.Register;
import java.util.Arrays;

class Vcnl4010Registers extends RegisterManager
{
    Vcnl4010Registers(IRegisterManagerAdapter adapter)
    {
        super(adapter);
        command = new CommandRegister(this);
        _registers.add(command);
        productId = new ProductIdRegister(this);
        _registers.add(productId);
        proximityRate = new ProximityRateRegister(this);
        _registers.add(proximityRate);
        ledCurrent = new LedCurrentRegister(this);
        _registers.add(ledCurrent);
        ambientLightParameters = new AmbientLightParametersRegister(this);
        _registers.add(ambientLightParameters);
        ambientLightResult = new AmbientLightResultRegister(this);
        _registers.add(ambientLightResult);
        proximityResult = new ProximityResultRegister(this);
        _registers.add(proximityResult);
        interruptControl = new InterruptControlRegister(this);
        _registers.add(interruptControl);
        lowThreshold = new LowThresholdRegister(this);
        _registers.add(lowThreshold);
        highThreshold = new HighThresholdRegister(this);
        _registers.add(highThreshold);
        interruptStatus = new InterruptStatusRegister(this);
        _registers.add(interruptStatus);
        proxModulatorTimingAdustment = new ProxModulatorTimingAdustmentRegister(this);
        _registers.add(proxModulatorTimingAdustment);
    }

    CommandRegister command;
    ProductIdRegister productId;
    ProximityRateRegister proximityRate;
    LedCurrentRegister ledCurrent;
    AmbientLightParametersRegister ambientLightParameters;
    AmbientLightResultRegister ambientLightResult;
    ProximityResultRegister proximityResult;
    InterruptControlRegister interruptControl;
    LowThresholdRegister lowThreshold;
    HighThresholdRegister highThreshold;
    InterruptStatusRegister interruptStatus;
    ProxModulatorTimingAdustmentRegister proxModulatorTimingAdustment;

    class CommandRegister extends Register
    {
        CommandRegister(RegisterManager regManager) { super(regManager, 0x80, 1, false); }

        int selfTimedEnable;
        int proxPeriodicEnable;
        int alsPeriodicEnable;
        int proxOnDemandStart;
        int alsOnDemandStart;
        int proxDataReady;
        int alsDataReady;
        int configLock;


        public CommandRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((selfTimedEnable & 0x1) << 0) | ((proxPeriodicEnable & 0x1) << 1) | ((alsPeriodicEnable & 0x1) << 2) | ((proxOnDemandStart & 0x1) << 3) | ((alsOnDemandStart & 0x1) << 4) | ((proxDataReady & 0x1) << 5) | ((alsDataReady & 0x1) << 6) | ((configLock & 0x1) << 7); }
        public void setValue(long _value)
        {
            selfTimedEnable = (int)((_value >> 0) & 0x1);
            proxPeriodicEnable = (int)((_value >> 1) & 0x1);
            alsPeriodicEnable = (int)((_value >> 2) & 0x1);
            proxOnDemandStart = (int)((_value >> 3) & 0x1);
            alsOnDemandStart = (int)((_value >> 4) & 0x1);
            proxDataReady = (int)((_value >> 5) & 0x1);
            alsDataReady = (int)((_value >> 6) & 0x1);
            configLock = (int)((_value >> 7) & 0x1);
        }
    }
    class ProductIdRegister extends Register
    {
        ProductIdRegister(RegisterManager regManager) { super(regManager, 0x82, 1, false); }

        int revisionId;
        int productId;


        public ProductIdRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((revisionId & 0xF) << 0) | ((productId & 0xF) << 4); }
        public void setValue(long _value)
        {
            revisionId = (int)((_value >> 0) & 0xF);
            productId = (int)((_value >> 4) & 0xF);
        }
    }
    class ProximityRateRegister extends Register
    {
        ProximityRateRegister(RegisterManager regManager) { super(regManager, 0x82, 1, false); }

        int rate;

                public Rates getRate() { for (Rates b : Rates.values()) { if(b.getVal() == rate) return b; } return Rates.values()[0]; }
                public void setRate(Rates enumVal) { rate = enumVal.getVal(); }

        public ProximityRateRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((rate & 0xF) << 0); }
        public void setValue(long _value)
        {
            rate = (int)((_value >> 0) & 0xF);
        }
    }
    class LedCurrentRegister extends Register
    {
        LedCurrentRegister(RegisterManager regManager) { super(regManager, 0x83, 1, false); }

        int irLedCurrentValue;
        int fuseProgId;


        public LedCurrentRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((irLedCurrentValue & 0x3F) << 0) | ((fuseProgId & 0x3) << 6); }
        public void setValue(long _value)
        {
            irLedCurrentValue = (int)((_value >> 0) & 0x3F);
            fuseProgId = (int)((_value >> 6) & 0x3);
        }
    }
    class AmbientLightParametersRegister extends Register
    {
        AmbientLightParametersRegister(RegisterManager regManager) { super(regManager, 0x84, 1, false); }

        int averagingSamples;
        int autoOffsetCompensation;
        int alsRate;
        int continuousConversionMode;

                public AlsRates getAlsRate() { for (AlsRates b : AlsRates.values()) { if(b.getVal() == alsRate) return b; } return AlsRates.values()[0]; }
                public void setAlsRate(AlsRates enumVal) { alsRate = enumVal.getVal(); }

        public AmbientLightParametersRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((averagingSamples & 0x7) << 0) | ((autoOffsetCompensation & 0x1) << 3) | ((alsRate & 0x7) << 4) | ((continuousConversionMode & 0x1) << 7); }
        public void setValue(long _value)
        {
            averagingSamples = (int)((_value >> 0) & 0x7);
            autoOffsetCompensation = (int)((_value >> 3) & 0x1);
            alsRate = (int)((_value >> 4) & 0x7);
            continuousConversionMode = (int)((_value >> 7) & 0x1);
        }
    }
    class AmbientLightResultRegister extends Register
    {
        AmbientLightResultRegister(RegisterManager regManager) { super(regManager, 0x85, 2, false); }

        int value;


        public AmbientLightResultRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((value & 0xFFFF) << 0); }
        public void setValue(long _value)
        {
            value = (int)((_value >> 0) & 0xFFFF);
        }
    }
    class ProximityResultRegister extends Register
    {
        ProximityResultRegister(RegisterManager regManager) { super(regManager, 0x87, 2, true); }

        int value;


        public ProximityResultRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((value & 0xFFFF) << 0); }
        public void setValue(long _value)
        {
            value = (int)((_value >> 0) & 0xFFFF);
        }
    }
    class InterruptControlRegister extends Register
    {
        InterruptControlRegister(RegisterManager regManager) { super(regManager, 0x89, 1, false); }

        int interruptThresholdSelect;
        int interruptThresholdEnable;
        int interruptAlsReadyEnable;
        int intCountExceed;

                public IntCountExceeds getIntCountExceed() { for (IntCountExceeds b : IntCountExceeds.values()) { if(b.getVal() == intCountExceed) return b; } return IntCountExceeds.values()[0]; }
                public void setIntCountExceed(IntCountExceeds enumVal) { intCountExceed = enumVal.getVal(); }

        public InterruptControlRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((interruptThresholdSelect & 0x1) << 0) | ((interruptThresholdEnable & 0x1) << 1) | ((interruptAlsReadyEnable & 0x1) << 2) | ((intCountExceed & 0x7) << 5); }
        public void setValue(long _value)
        {
            interruptThresholdSelect = (int)((_value >> 0) & 0x1);
            interruptThresholdEnable = (int)((_value >> 1) & 0x1);
            interruptAlsReadyEnable = (int)((_value >> 2) & 0x1);
            intCountExceed = (int)((_value >> 5) & 0x7);
        }
    }
    class LowThresholdRegister extends Register
    {
        LowThresholdRegister(RegisterManager regManager) { super(regManager, 0x8A, 2, true); }

        int value;


        public LowThresholdRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((value & 0xFFFF) << 0); }
        public void setValue(long _value)
        {
            value = (int)((_value >> 0) & 0xFFFF);
        }
    }
    class HighThresholdRegister extends Register
    {
        HighThresholdRegister(RegisterManager regManager) { super(regManager, 0x8C, 2, true); }

        int value;


        public HighThresholdRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((value & 0xFFFF) << 0); }
        public void setValue(long _value)
        {
            value = (int)((_value >> 0) & 0xFFFF);
        }
    }
    class InterruptStatusRegister extends Register
    {
        InterruptStatusRegister(RegisterManager regManager) { super(regManager, 0x8E, 1, false); }

        int intThresholdHighExceeded;
        int intThresholdLowExceeded;
        int intAlsReady;
        int intProxReady;


        public InterruptStatusRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((intThresholdHighExceeded & 0x1) << 0) | ((intThresholdLowExceeded & 0x1) << 1) | ((intAlsReady & 0x1) << 2) | ((intProxReady & 0x1) << 3); }
        public void setValue(long _value)
        {
            intThresholdHighExceeded = (int)((_value >> 0) & 0x1);
            intThresholdLowExceeded = (int)((_value >> 1) & 0x1);
            intAlsReady = (int)((_value >> 2) & 0x1);
            intProxReady = (int)((_value >> 3) & 0x1);
        }
    }
    class ProxModulatorTimingAdustmentRegister extends Register
    {
        ProxModulatorTimingAdustmentRegister(RegisterManager regManager) { super(regManager, 0x8F, 1, false); }

        int modulationDeadTime;
        int proximityFrequency;
        int modulationDelayTime;


        public ProxModulatorTimingAdustmentRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((modulationDeadTime & 0x7) << 0) | ((proximityFrequency & 0x3) << 3) | ((modulationDelayTime & 0x7) << 5); }
        public void setValue(long _value)
        {
            modulationDeadTime = (int)((_value >> 0) & 0x7);
            proximityFrequency = (int)((_value >> 3) & 0x3);
            modulationDelayTime = (int)((_value >> 5) & 0x7);
        }
    }
}
