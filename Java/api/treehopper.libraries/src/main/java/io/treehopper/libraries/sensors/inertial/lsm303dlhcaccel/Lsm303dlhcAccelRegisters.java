/// This file was auto-generated by RegisterGenerator. Any changes to it will be overwritten!
package io.treehopper.libraries.sensors.inertial.lsm303dlhcaccel;

import io.treehopper.libraries.IRegisterManagerAdapter;
import io.treehopper.libraries.RegisterManager;
import io.treehopper.libraries.Register;
import java.util.Arrays;

class Lsm303dlhcAccelRegisters extends RegisterManager
{
    Lsm303dlhcAccelRegisters(IRegisterManagerAdapter adapter)
    {
        super(adapter);
        ctrl1 = new Ctrl1Register(this);
        _registers.add(ctrl1);
        ctrl2 = new Ctrl2Register(this);
        _registers.add(ctrl2);
        ctrl3 = new Ctrl3Register(this);
        _registers.add(ctrl3);
        ctrl4 = new Ctrl4Register(this);
        _registers.add(ctrl4);
        ctrl5 = new Ctrl5Register(this);
        _registers.add(ctrl5);
        ctrl6 = new Ctrl6Register(this);
        _registers.add(ctrl6);
        reference = new ReferenceRegister(this);
        _registers.add(reference);
        status = new StatusRegister(this);
        _registers.add(status);
        fifoControl = new FifoControlRegister(this);
        _registers.add(fifoControl);
        fifoSource = new FifoSourceRegister(this);
        _registers.add(fifoSource);
        inertialIntGen1Config = new InertialIntGen1ConfigRegister(this);
        _registers.add(inertialIntGen1Config);
        inertialIntGen1Status = new InertialIntGen1StatusRegister(this);
        _registers.add(inertialIntGen1Status);
        inertialIntGen1Threshold = new InertialIntGen1ThresholdRegister(this);
        _registers.add(inertialIntGen1Threshold);
        inertialIntGen1Duration = new InertialIntGen1DurationRegister(this);
        _registers.add(inertialIntGen1Duration);
        inertialIntGen2Config = new InertialIntGen2ConfigRegister(this);
        _registers.add(inertialIntGen2Config);
        inertialIntGen2Status = new InertialIntGen2StatusRegister(this);
        _registers.add(inertialIntGen2Status);
        inertialIntGen2Threshold = new InertialIntGen2ThresholdRegister(this);
        _registers.add(inertialIntGen2Threshold);
        inertialIntGen2Duration = new InertialIntGen2DurationRegister(this);
        _registers.add(inertialIntGen2Duration);
        clickConfig = new ClickConfigRegister(this);
        _registers.add(clickConfig);
        clickSource = new ClickSourceRegister(this);
        _registers.add(clickSource);
        clickThreshold = new ClickThresholdRegister(this);
        _registers.add(clickThreshold);
        timeLimit = new TimeLimitRegister(this);
        _registers.add(timeLimit);
        timeLatency = new TimeLatencyRegister(this);
        _registers.add(timeLatency);
        timeWindow = new TimeWindowRegister(this);
        _registers.add(timeWindow);
        outAccelX = new OutAccelXRegister(this);
        _registers.add(outAccelX);
        outAccelY = new OutAccelYRegister(this);
        _registers.add(outAccelY);
        outAccelZ = new OutAccelZRegister(this);
        _registers.add(outAccelZ);
    }

    Ctrl1Register ctrl1;
    Ctrl2Register ctrl2;
    Ctrl3Register ctrl3;
    Ctrl4Register ctrl4;
    Ctrl5Register ctrl5;
    Ctrl6Register ctrl6;
    ReferenceRegister reference;
    StatusRegister status;
    FifoControlRegister fifoControl;
    FifoSourceRegister fifoSource;
    InertialIntGen1ConfigRegister inertialIntGen1Config;
    InertialIntGen1StatusRegister inertialIntGen1Status;
    InertialIntGen1ThresholdRegister inertialIntGen1Threshold;
    InertialIntGen1DurationRegister inertialIntGen1Duration;
    InertialIntGen2ConfigRegister inertialIntGen2Config;
    InertialIntGen2StatusRegister inertialIntGen2Status;
    InertialIntGen2ThresholdRegister inertialIntGen2Threshold;
    InertialIntGen2DurationRegister inertialIntGen2Duration;
    ClickConfigRegister clickConfig;
    ClickSourceRegister clickSource;
    ClickThresholdRegister clickThreshold;
    TimeLimitRegister timeLimit;
    TimeLatencyRegister timeLatency;
    TimeWindowRegister timeWindow;
    OutAccelXRegister outAccelX;
    OutAccelYRegister outAccelY;
    OutAccelZRegister outAccelZ;

    class Ctrl1Register extends Register
    {
        Ctrl1Register(RegisterManager regManager) { super(regManager, 0x20, 1, false); }

        int xEnable;
        int yEnable;
        int zEnable;
        int lowPowerEnable;
        int outputDataRate;

                public OutputDataRates getOutputDataRate() { for (OutputDataRates b : OutputDataRates.values()) { if(b.getVal() == outputDataRate) return b; } return OutputDataRates.values()[0]; }
                public void setOutputDataRate(OutputDataRates enumVal) { outputDataRate = enumVal.getVal(); }

        public Ctrl1Register read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((xEnable & 0x1) << 0) | ((yEnable & 0x1) << 1) | ((zEnable & 0x1) << 2) | ((lowPowerEnable & 0x1) << 3) | ((outputDataRate & 0xF) << 4); }
        public void setValue(long _value)
        {
            xEnable = (int)((_value >> 0) & 0x1);
            yEnable = (int)((_value >> 1) & 0x1);
            zEnable = (int)((_value >> 2) & 0x1);
            lowPowerEnable = (int)((_value >> 3) & 0x1);
            outputDataRate = (int)((_value >> 4) & 0xF);
        }
    }
    class Ctrl2Register extends Register
    {
        Ctrl2Register(RegisterManager regManager) { super(regManager, 0x21, 1, false); }

        int hpis;
        int hpClick;
        int filterDataSelection;
        int hpcf;
        int accelhighPassMode;

                public AccelhighPassModes getAccelhighPassMode() { for (AccelhighPassModes b : AccelhighPassModes.values()) { if(b.getVal() == accelhighPassMode) return b; } return AccelhighPassModes.values()[0]; }
                public void setAccelhighPassMode(AccelhighPassModes enumVal) { accelhighPassMode = enumVal.getVal(); }

        public Ctrl2Register read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((hpis & 0x3) << 0) | ((hpClick & 0x1) << 2) | ((filterDataSelection & 0x1) << 3) | ((hpcf & 0x3) << 4) | ((accelhighPassMode & 0x3) << 6); }
        public void setValue(long _value)
        {
            hpis = (int)((_value >> 0) & 0x3);
            hpClick = (int)((_value >> 2) & 0x1);
            filterDataSelection = (int)((_value >> 3) & 0x1);
            hpcf = (int)((_value >> 4) & 0x3);
            accelhighPassMode = (int)((_value >> 6) & 0x3);
        }
    }
    class Ctrl3Register extends Register
    {
        Ctrl3Register(RegisterManager regManager) { super(regManager, 0x22, 1, false); }

        int fifoOverrunOnInt1;
        int fifoWatermarkOnInt1;
        int drdy2OnInt1;
        int drdy1OnInt1;
        int aoi2OnInt1;
        int aoi1OnInt1;
        int clickOnInt2;


        public Ctrl3Register read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((fifoOverrunOnInt1 & 0x1) << 1) | ((fifoWatermarkOnInt1 & 0x1) << 2) | ((drdy2OnInt1 & 0x1) << 3) | ((drdy1OnInt1 & 0x1) << 4) | ((aoi2OnInt1 & 0x1) << 5) | ((aoi1OnInt1 & 0x1) << 6) | ((clickOnInt2 & 0x1) << 7); }
        public void setValue(long _value)
        {
            fifoOverrunOnInt1 = (int)((_value >> 1) & 0x1);
            fifoWatermarkOnInt1 = (int)((_value >> 2) & 0x1);
            drdy2OnInt1 = (int)((_value >> 3) & 0x1);
            drdy1OnInt1 = (int)((_value >> 4) & 0x1);
            aoi2OnInt1 = (int)((_value >> 5) & 0x1);
            aoi1OnInt1 = (int)((_value >> 6) & 0x1);
            clickOnInt2 = (int)((_value >> 7) & 0x1);
        }
    }
    class Ctrl4Register extends Register
    {
        Ctrl4Register(RegisterManager regManager) { super(regManager, 0x23, 1, false); }

        int spiModeSelection;
        int highResolution;
        int fullScaleSelection;
        int ble;
        int bdu;

                public FullScaleSelections getFullScaleSelection() { for (FullScaleSelections b : FullScaleSelections.values()) { if(b.getVal() == fullScaleSelection) return b; } return FullScaleSelections.values()[0]; }
                public void setFullScaleSelection(FullScaleSelections enumVal) { fullScaleSelection = enumVal.getVal(); }

        public Ctrl4Register read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((spiModeSelection & 0x1) << 0) | ((highResolution & 0x1) << 3) | ((fullScaleSelection & 0x7) << 3) | ((ble & 0x1) << 6) | ((bdu & 0x1) << 7); }
        public void setValue(long _value)
        {
            spiModeSelection = (int)((_value >> 0) & 0x1);
            highResolution = (int)((_value >> 3) & 0x1);
            fullScaleSelection = (int)((_value >> 3) & 0x7);
            ble = (int)((_value >> 6) & 0x1);
            bdu = (int)((_value >> 7) & 0x1);
        }
    }
    class Ctrl5Register extends Register
    {
        Ctrl5Register(RegisterManager regManager) { super(regManager, 0x24, 1, false); }

        int d4dInt2;
        int lirInt2;
        int d4dInt1;
        int lirInt1;
        int fifoEnable;
        int boot;


        public Ctrl5Register read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((d4dInt2 & 0x1) << 0) | ((lirInt2 & 0x1) << 1) | ((d4dInt1 & 0x1) << 2) | ((lirInt1 & 0x1) << 3) | ((fifoEnable & 0x1) << 6) | ((boot & 0x1) << 7); }
        public void setValue(long _value)
        {
            d4dInt2 = (int)((_value >> 0) & 0x1);
            lirInt2 = (int)((_value >> 1) & 0x1);
            d4dInt1 = (int)((_value >> 2) & 0x1);
            lirInt1 = (int)((_value >> 3) & 0x1);
            fifoEnable = (int)((_value >> 6) & 0x1);
            boot = (int)((_value >> 7) & 0x1);
        }
    }
    class Ctrl6Register extends Register
    {
        Ctrl6Register(RegisterManager regManager) { super(regManager, 0x25, 1, false); }

        int interruptActiveHigh;
        int p2Act;
        int bootI1;
        int i2Int2;
        int i2Int1;
        int i2ClickEnable;


        public Ctrl6Register read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((interruptActiveHigh & 0x1) << 1) | ((p2Act & 0x1) << 3) | ((bootI1 & 0x1) << 4) | ((i2Int2 & 0x1) << 5) | ((i2Int1 & 0x1) << 6) | ((i2ClickEnable & 0x1) << 7); }
        public void setValue(long _value)
        {
            interruptActiveHigh = (int)((_value >> 1) & 0x1);
            p2Act = (int)((_value >> 3) & 0x1);
            bootI1 = (int)((_value >> 4) & 0x1);
            i2Int2 = (int)((_value >> 5) & 0x1);
            i2Int1 = (int)((_value >> 6) & 0x1);
            i2ClickEnable = (int)((_value >> 7) & 0x1);
        }
    }
    class ReferenceRegister extends Register
    {
        ReferenceRegister(RegisterManager regManager) { super(regManager, 0x26, 1, false); }

        int value;


        public ReferenceRegister read()
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
    class StatusRegister extends Register
    {
        StatusRegister(RegisterManager regManager) { super(regManager, 0x27, 1, false); }

        int xDataAvailable;
        int yDataAvailable;
        int zDataAvailable;
        int zyxDataAvailable;
        int xDataOverrun;
        int yDataOverrun;
        int zDataOverrun;
        int zyxDataOverrun;


        public StatusRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((xDataAvailable & 0x1) << 0) | ((yDataAvailable & 0x1) << 1) | ((zDataAvailable & 0x1) << 2) | ((zyxDataAvailable & 0x1) << 3) | ((xDataOverrun & 0x1) << 4) | ((yDataOverrun & 0x1) << 5) | ((zDataOverrun & 0x1) << 6) | ((zyxDataOverrun & 0x1) << 7); }
        public void setValue(long _value)
        {
            xDataAvailable = (int)((_value >> 0) & 0x1);
            yDataAvailable = (int)((_value >> 1) & 0x1);
            zDataAvailable = (int)((_value >> 2) & 0x1);
            zyxDataAvailable = (int)((_value >> 3) & 0x1);
            xDataOverrun = (int)((_value >> 4) & 0x1);
            yDataOverrun = (int)((_value >> 5) & 0x1);
            zDataOverrun = (int)((_value >> 6) & 0x1);
            zyxDataOverrun = (int)((_value >> 7) & 0x1);
        }
    }
    class FifoControlRegister extends Register
    {
        FifoControlRegister(RegisterManager regManager) { super(regManager, 0x2E, 1, false); }

        int fifoThreshold;
        int fifoMode;

                public FifoModes getFifoMode() { for (FifoModes b : FifoModes.values()) { if(b.getVal() == fifoMode) return b; } return FifoModes.values()[0]; }
                public void setFifoMode(FifoModes enumVal) { fifoMode = enumVal.getVal(); }

        public FifoControlRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((fifoThreshold & 0x1F) << 0) | ((fifoMode & 0x7) << 5); }
        public void setValue(long _value)
        {
            fifoThreshold = (int)((_value >> 0) & 0x1F);
            fifoMode = (int)((_value >> 5) & 0x7);
        }
    }
    class FifoSourceRegister extends Register
    {
        FifoSourceRegister(RegisterManager regManager) { super(regManager, 0x2f, 1, false); }

        int fifoStoredLevel;
        int empty;
        int overrun;
        int fifoThreshold;


        public FifoSourceRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((fifoStoredLevel & 0x1F) << 0) | ((empty & 0x1) << 5) | ((overrun & 0x1) << 6) | ((fifoThreshold & 0x1) << 7); }
        public void setValue(long _value)
        {
            fifoStoredLevel = (int)((_value >> 0) & 0x1F);
            empty = (int)((_value >> 5) & 0x1);
            overrun = (int)((_value >> 6) & 0x1);
            fifoThreshold = (int)((_value >> 7) & 0x1);
        }
    }
    class InertialIntGen1ConfigRegister extends Register
    {
        InertialIntGen1ConfigRegister(RegisterManager regManager) { super(regManager, 0x30, 1, false); }

        int xLowInterruptEnable;
        int xHighInterruptEnable;
        int yLowInterruptEnable;
        int yHighInterruptEnable;
        int zLowInterruptEvent;
        int zHighInterruptEnable;
        int detect6D;
        int andOrInterruptEvents;


        public InertialIntGen1ConfigRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((xLowInterruptEnable & 0x1) << 0) | ((xHighInterruptEnable & 0x1) << 1) | ((yLowInterruptEnable & 0x1) << 2) | ((yHighInterruptEnable & 0x1) << 3) | ((zLowInterruptEvent & 0x1) << 4) | ((zHighInterruptEnable & 0x1) << 5) | ((detect6D & 0x1) << 6) | ((andOrInterruptEvents & 0x1) << 7); }
        public void setValue(long _value)
        {
            xLowInterruptEnable = (int)((_value >> 0) & 0x1);
            xHighInterruptEnable = (int)((_value >> 1) & 0x1);
            yLowInterruptEnable = (int)((_value >> 2) & 0x1);
            yHighInterruptEnable = (int)((_value >> 3) & 0x1);
            zLowInterruptEvent = (int)((_value >> 4) & 0x1);
            zHighInterruptEnable = (int)((_value >> 5) & 0x1);
            detect6D = (int)((_value >> 6) & 0x1);
            andOrInterruptEvents = (int)((_value >> 7) & 0x1);
        }
    }
    class InertialIntGen1StatusRegister extends Register
    {
        InertialIntGen1StatusRegister(RegisterManager regManager) { super(regManager, 0x31, 1, false); }

        int xLow;
        int xHigh;
        int yLow;
        int yHigh;
        int zLow;
        int zHigh;
        int intStatus;


        public InertialIntGen1StatusRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((xLow & 0x1) << 0) | ((xHigh & 0x1) << 1) | ((yLow & 0x1) << 2) | ((yHigh & 0x1) << 3) | ((zLow & 0x1) << 4) | ((zHigh & 0x1) << 5) | ((intStatus & 0x1) << 6); }
        public void setValue(long _value)
        {
            xLow = (int)((_value >> 0) & 0x1);
            xHigh = (int)((_value >> 1) & 0x1);
            yLow = (int)((_value >> 2) & 0x1);
            yHigh = (int)((_value >> 3) & 0x1);
            zLow = (int)((_value >> 4) & 0x1);
            zHigh = (int)((_value >> 5) & 0x1);
            intStatus = (int)((_value >> 6) & 0x1);
        }
    }
    class InertialIntGen1ThresholdRegister extends Register
    {
        InertialIntGen1ThresholdRegister(RegisterManager regManager) { super(regManager, 0x32, 1, false); }

        int value;


        public InertialIntGen1ThresholdRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((value & 0x7F) << 0); }
        public void setValue(long _value)
        {
            value = (int)((_value >> 0) & 0x7F);
        }
    }
    class InertialIntGen1DurationRegister extends Register
    {
        InertialIntGen1DurationRegister(RegisterManager regManager) { super(regManager, 0x33, 1, false); }

        int value;


        public InertialIntGen1DurationRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((value & 0x7F) << 0); }
        public void setValue(long _value)
        {
            value = (int)((_value >> 0) & 0x7F);
        }
    }
    class InertialIntGen2ConfigRegister extends Register
    {
        InertialIntGen2ConfigRegister(RegisterManager regManager) { super(regManager, 0x34, 1, false); }

        int xLowInterruptEnable;
        int xHighInterruptEnable;
        int yLowInterruptEnable;
        int yHighInterruptEnable;
        int zLowInterruptEvent;
        int zHighInterruptEnable;
        int detect6D;
        int andOrInterruptEvents;


        public InertialIntGen2ConfigRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((xLowInterruptEnable & 0x1) << 0) | ((xHighInterruptEnable & 0x1) << 1) | ((yLowInterruptEnable & 0x1) << 2) | ((yHighInterruptEnable & 0x1) << 3) | ((zLowInterruptEvent & 0x1) << 4) | ((zHighInterruptEnable & 0x1) << 5) | ((detect6D & 0x1) << 6) | ((andOrInterruptEvents & 0x1) << 7); }
        public void setValue(long _value)
        {
            xLowInterruptEnable = (int)((_value >> 0) & 0x1);
            xHighInterruptEnable = (int)((_value >> 1) & 0x1);
            yLowInterruptEnable = (int)((_value >> 2) & 0x1);
            yHighInterruptEnable = (int)((_value >> 3) & 0x1);
            zLowInterruptEvent = (int)((_value >> 4) & 0x1);
            zHighInterruptEnable = (int)((_value >> 5) & 0x1);
            detect6D = (int)((_value >> 6) & 0x1);
            andOrInterruptEvents = (int)((_value >> 7) & 0x1);
        }
    }
    class InertialIntGen2StatusRegister extends Register
    {
        InertialIntGen2StatusRegister(RegisterManager regManager) { super(regManager, 0x35, 1, false); }

        int xLow;
        int xHigh;
        int yLow;
        int yHigh;
        int zLow;
        int zHigh;
        int intStatus;


        public InertialIntGen2StatusRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((xLow & 0x1) << 0) | ((xHigh & 0x1) << 1) | ((yLow & 0x1) << 2) | ((yHigh & 0x1) << 3) | ((zLow & 0x1) << 4) | ((zHigh & 0x1) << 5) | ((intStatus & 0x1) << 6); }
        public void setValue(long _value)
        {
            xLow = (int)((_value >> 0) & 0x1);
            xHigh = (int)((_value >> 1) & 0x1);
            yLow = (int)((_value >> 2) & 0x1);
            yHigh = (int)((_value >> 3) & 0x1);
            zLow = (int)((_value >> 4) & 0x1);
            zHigh = (int)((_value >> 5) & 0x1);
            intStatus = (int)((_value >> 6) & 0x1);
        }
    }
    class InertialIntGen2ThresholdRegister extends Register
    {
        InertialIntGen2ThresholdRegister(RegisterManager regManager) { super(regManager, 0x36, 1, false); }

        int value;


        public InertialIntGen2ThresholdRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((value & 0x7F) << 0); }
        public void setValue(long _value)
        {
            value = (int)((_value >> 0) & 0x7F);
        }
    }
    class InertialIntGen2DurationRegister extends Register
    {
        InertialIntGen2DurationRegister(RegisterManager regManager) { super(regManager, 0x37, 1, false); }

        int value;


        public InertialIntGen2DurationRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((value & 0x7F) << 0); }
        public void setValue(long _value)
        {
            value = (int)((_value >> 0) & 0x7F);
        }
    }
    class ClickConfigRegister extends Register
    {
        ClickConfigRegister(RegisterManager regManager) { super(regManager, 0x38, 1, false); }

        int xSingleClick;
        int xDoubleClick;
        int ySingleClick;
        int yDoubleClick;
        int zSingleClick;
        int zDoubleClick;


        public ClickConfigRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((xSingleClick & 0x1) << 0) | ((xDoubleClick & 0x1) << 1) | ((ySingleClick & 0x1) << 2) | ((yDoubleClick & 0x1) << 3) | ((zSingleClick & 0x1) << 4) | ((zDoubleClick & 0x1) << 5); }
        public void setValue(long _value)
        {
            xSingleClick = (int)((_value >> 0) & 0x1);
            xDoubleClick = (int)((_value >> 1) & 0x1);
            ySingleClick = (int)((_value >> 2) & 0x1);
            yDoubleClick = (int)((_value >> 3) & 0x1);
            zSingleClick = (int)((_value >> 4) & 0x1);
            zDoubleClick = (int)((_value >> 5) & 0x1);
        }
    }
    class ClickSourceRegister extends Register
    {
        ClickSourceRegister(RegisterManager regManager) { super(regManager, 0x39, 1, false); }

        int x;
        int y;
        int z;
        int sign;
        int singleClickEn;
        int doubleClickEn;
        int interruptActive;


        public ClickSourceRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((x & 0x1) << 0) | ((y & 0x1) << 1) | ((z & 0x1) << 2) | ((sign & 0x1) << 3) | ((singleClickEn & 0x1) << 4) | ((doubleClickEn & 0x1) << 5) | ((interruptActive & 0x1) << 6); }
        public void setValue(long _value)
        {
            x = (int)((_value >> 0) & 0x1);
            y = (int)((_value >> 1) & 0x1);
            z = (int)((_value >> 2) & 0x1);
            sign = (int)((_value >> 3) & 0x1);
            singleClickEn = (int)((_value >> 4) & 0x1);
            doubleClickEn = (int)((_value >> 5) & 0x1);
            interruptActive = (int)((_value >> 6) & 0x1);
        }
    }
    class ClickThresholdRegister extends Register
    {
        ClickThresholdRegister(RegisterManager regManager) { super(regManager, 0x3A, 1, false); }

        int value;


        public ClickThresholdRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((value & 0x7F) << 0); }
        public void setValue(long _value)
        {
            value = (int)((_value >> 0) & 0x7F);
        }
    }
    class TimeLimitRegister extends Register
    {
        TimeLimitRegister(RegisterManager regManager) { super(regManager, 0x3b, 1, false); }

        int value;


        public TimeLimitRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((value & 0x7F) << 0); }
        public void setValue(long _value)
        {
            value = (int)((_value >> 0) & 0x7F);
        }
    }
    class TimeLatencyRegister extends Register
    {
        TimeLatencyRegister(RegisterManager regManager) { super(regManager, 0x3c, 1, false); }

        int value;


        public TimeLatencyRegister read()
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
    class TimeWindowRegister extends Register
    {
        TimeWindowRegister(RegisterManager regManager) { super(regManager, 0x3d, 1, false); }

        int value;


        public TimeWindowRegister read()
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
    class OutAccelXRegister extends Register
    {
        OutAccelXRegister(RegisterManager regManager) { super(regManager, 0xa8, 2, false); }

        int value;


        public OutAccelXRegister read()
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
    class OutAccelYRegister extends Register
    {
        OutAccelYRegister(RegisterManager regManager) { super(regManager, 0xaA, 2, false); }

        int value;


        public OutAccelYRegister read()
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
    class OutAccelZRegister extends Register
    {
        OutAccelZRegister(RegisterManager regManager) { super(regManager, 0xaC, 2, false); }

        int value;


        public OutAccelZRegister read()
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
