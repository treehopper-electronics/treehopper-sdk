/// This file was auto-generated by RegisterGenerator. Any changes to it will be overwritten!
package io.treehopper.libraries.sensors.inertial.bno055;

import io.treehopper.libraries.IRegisterManagerAdapter;
import io.treehopper.libraries.RegisterManager;
import io.treehopper.libraries.Register;
import java.util.Arrays;

class Bno055Registers extends RegisterManager
{
    Bno055Registers(IRegisterManagerAdapter adapter)
    {
        super(adapter);
        chipId = new ChipIdRegister(this);
        _registers.add(chipId);
        accelChipId = new AccelChipIdRegister(this);
        _registers.add(accelChipId);
        magChipId = new MagChipIdRegister(this);
        _registers.add(magChipId);
        gyroChipId = new GyroChipIdRegister(this);
        _registers.add(gyroChipId);
        swRevision = new SwRevisionRegister(this);
        _registers.add(swRevision);
        bootloaderVersion = new BootloaderVersionRegister(this);
        _registers.add(bootloaderVersion);
        pageId = new PageIdRegister(this);
        _registers.add(pageId);
        accelX = new AccelXRegister(this);
        _registers.add(accelX);
        accelY = new AccelYRegister(this);
        _registers.add(accelY);
        accelZ = new AccelZRegister(this);
        _registers.add(accelZ);
        magnetometerX = new MagnetometerXRegister(this);
        _registers.add(magnetometerX);
        magnetometerY = new MagnetometerYRegister(this);
        _registers.add(magnetometerY);
        magnetometerZ = new MagnetometerZRegister(this);
        _registers.add(magnetometerZ);
        gyroX = new GyroXRegister(this);
        _registers.add(gyroX);
        gyroY = new GyroYRegister(this);
        _registers.add(gyroY);
        gyroZ = new GyroZRegister(this);
        _registers.add(gyroZ);
        eulHeading = new EulHeadingRegister(this);
        _registers.add(eulHeading);
        eulRoll = new EulRollRegister(this);
        _registers.add(eulRoll);
        eulPitch = new EulPitchRegister(this);
        _registers.add(eulPitch);
        quaW = new QuaWRegister(this);
        _registers.add(quaW);
        quaX = new QuaXRegister(this);
        _registers.add(quaX);
        quaY = new QuaYRegister(this);
        _registers.add(quaY);
        quaZ = new QuaZRegister(this);
        _registers.add(quaZ);
        linX = new LinXRegister(this);
        _registers.add(linX);
        linY = new LinYRegister(this);
        _registers.add(linY);
        linZ = new LinZRegister(this);
        _registers.add(linZ);
        gravX = new GravXRegister(this);
        _registers.add(gravX);
        gravY = new GravYRegister(this);
        _registers.add(gravY);
        gravZ = new GravZRegister(this);
        _registers.add(gravZ);
        temp = new TempRegister(this);
        _registers.add(temp);
        calibStat = new CalibStatRegister(this);
        _registers.add(calibStat);
        selfTestResult = new SelfTestResultRegister(this);
        _registers.add(selfTestResult);
        interruptStatus = new InterruptStatusRegister(this);
        _registers.add(interruptStatus);
        sysClockStatus = new SysClockStatusRegister(this);
        _registers.add(sysClockStatus);
        sysStatus = new SysStatusRegister(this);
        _registers.add(sysStatus);
        sysErr = new SysErrRegister(this);
        _registers.add(sysErr);
        unitSel = new UnitSelRegister(this);
        _registers.add(unitSel);
        operatingMode = new OperatingModeRegister(this);
        _registers.add(operatingMode);
        powerMode = new PowerModeRegister(this);
        _registers.add(powerMode);
        sysTrigger = new SysTriggerRegister(this);
        _registers.add(sysTrigger);
        tempSource = new TempSourceRegister(this);
        _registers.add(tempSource);
        axisMapConfig = new AxisMapConfigRegister(this);
        _registers.add(axisMapConfig);
        axisMapSign = new AxisMapSignRegister(this);
        _registers.add(axisMapSign);
        accelOffsetX = new AccelOffsetXRegister(this);
        _registers.add(accelOffsetX);
        accelOffsetY = new AccelOffsetYRegister(this);
        _registers.add(accelOffsetY);
        accelOffsetZ = new AccelOffsetZRegister(this);
        _registers.add(accelOffsetZ);
        magnetometerOffsetX = new MagnetometerOffsetXRegister(this);
        _registers.add(magnetometerOffsetX);
        magnetometerOffsetY = new MagnetometerOffsetYRegister(this);
        _registers.add(magnetometerOffsetY);
        magnetometerOffsetZ = new MagnetometerOffsetZRegister(this);
        _registers.add(magnetometerOffsetZ);
        gyroOffsetX = new GyroOffsetXRegister(this);
        _registers.add(gyroOffsetX);
        gyroOffsetY = new GyroOffsetYRegister(this);
        _registers.add(gyroOffsetY);
        gyroOffsetZ = new GyroOffsetZRegister(this);
        _registers.add(gyroOffsetZ);
        accelRadius = new AccelRadiusRegister(this);
        _registers.add(accelRadius);
        magRadius = new MagRadiusRegister(this);
        _registers.add(magRadius);
    }

    ChipIdRegister chipId;
    AccelChipIdRegister accelChipId;
    MagChipIdRegister magChipId;
    GyroChipIdRegister gyroChipId;
    SwRevisionRegister swRevision;
    BootloaderVersionRegister bootloaderVersion;
    PageIdRegister pageId;
    AccelXRegister accelX;
    AccelYRegister accelY;
    AccelZRegister accelZ;
    MagnetometerXRegister magnetometerX;
    MagnetometerYRegister magnetometerY;
    MagnetometerZRegister magnetometerZ;
    GyroXRegister gyroX;
    GyroYRegister gyroY;
    GyroZRegister gyroZ;
    EulHeadingRegister eulHeading;
    EulRollRegister eulRoll;
    EulPitchRegister eulPitch;
    QuaWRegister quaW;
    QuaXRegister quaX;
    QuaYRegister quaY;
    QuaZRegister quaZ;
    LinXRegister linX;
    LinYRegister linY;
    LinZRegister linZ;
    GravXRegister gravX;
    GravYRegister gravY;
    GravZRegister gravZ;
    TempRegister temp;
    CalibStatRegister calibStat;
    SelfTestResultRegister selfTestResult;
    InterruptStatusRegister interruptStatus;
    SysClockStatusRegister sysClockStatus;
    SysStatusRegister sysStatus;
    SysErrRegister sysErr;
    UnitSelRegister unitSel;
    OperatingModeRegister operatingMode;
    PowerModeRegister powerMode;
    SysTriggerRegister sysTrigger;
    TempSourceRegister tempSource;
    AxisMapConfigRegister axisMapConfig;
    AxisMapSignRegister axisMapSign;
    AccelOffsetXRegister accelOffsetX;
    AccelOffsetYRegister accelOffsetY;
    AccelOffsetZRegister accelOffsetZ;
    MagnetometerOffsetXRegister magnetometerOffsetX;
    MagnetometerOffsetYRegister magnetometerOffsetY;
    MagnetometerOffsetZRegister magnetometerOffsetZ;
    GyroOffsetXRegister gyroOffsetX;
    GyroOffsetYRegister gyroOffsetY;
    GyroOffsetZRegister gyroOffsetZ;
    AccelRadiusRegister accelRadius;
    MagRadiusRegister magRadius;

    class ChipIdRegister extends Register
    {
        ChipIdRegister(RegisterManager regManager) { super(regManager, 0x00, 1, false); }

        int value;


        public ChipIdRegister read()
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
    class AccelChipIdRegister extends Register
    {
        AccelChipIdRegister(RegisterManager regManager) { super(regManager, 0x01, 1, false); }

        int value;


        public AccelChipIdRegister read()
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
    class MagChipIdRegister extends Register
    {
        MagChipIdRegister(RegisterManager regManager) { super(regManager, 0x02, 1, false); }

        int value;


        public MagChipIdRegister read()
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
    class GyroChipIdRegister extends Register
    {
        GyroChipIdRegister(RegisterManager regManager) { super(regManager, 0x03, 1, false); }

        int value;


        public GyroChipIdRegister read()
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
    class SwRevisionRegister extends Register
    {
        SwRevisionRegister(RegisterManager regManager) { super(regManager, 0x04, 2, false); }

        int value;


        public SwRevisionRegister read()
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
    class BootloaderVersionRegister extends Register
    {
        BootloaderVersionRegister(RegisterManager regManager) { super(regManager, 0x06, 1, false); }

        int value;


        public BootloaderVersionRegister read()
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
    class PageIdRegister extends Register
    {
        PageIdRegister(RegisterManager regManager) { super(regManager, 0x07, 1, false); }

        int value;


        public PageIdRegister read()
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
    class AccelXRegister extends Register
    {
        AccelXRegister(RegisterManager regManager) { super(regManager, 0x08, 2, false); }

        int value;


        public AccelXRegister read()
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
    class AccelYRegister extends Register
    {
        AccelYRegister(RegisterManager regManager) { super(regManager, 0x0A, 2, false); }

        int value;


        public AccelYRegister read()
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
    class AccelZRegister extends Register
    {
        AccelZRegister(RegisterManager regManager) { super(regManager, 0x0C, 2, false); }

        int value;


        public AccelZRegister read()
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
    class MagnetometerXRegister extends Register
    {
        MagnetometerXRegister(RegisterManager regManager) { super(regManager, 0x0E, 2, false); }

        int value;


        public MagnetometerXRegister read()
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
    class MagnetometerYRegister extends Register
    {
        MagnetometerYRegister(RegisterManager regManager) { super(regManager, 0x10, 2, false); }

        int value;


        public MagnetometerYRegister read()
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
    class MagnetometerZRegister extends Register
    {
        MagnetometerZRegister(RegisterManager regManager) { super(regManager, 0x12, 2, false); }

        int value;


        public MagnetometerZRegister read()
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
    class GyroXRegister extends Register
    {
        GyroXRegister(RegisterManager regManager) { super(regManager, 0x14, 2, false); }

        int value;


        public GyroXRegister read()
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
    class GyroYRegister extends Register
    {
        GyroYRegister(RegisterManager regManager) { super(regManager, 0x16, 2, false); }

        int value;


        public GyroYRegister read()
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
    class GyroZRegister extends Register
    {
        GyroZRegister(RegisterManager regManager) { super(regManager, 0x18, 2, false); }

        int value;


        public GyroZRegister read()
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
    class EulHeadingRegister extends Register
    {
        EulHeadingRegister(RegisterManager regManager) { super(regManager, 0x1A, 2, false); }

        int value;


        public EulHeadingRegister read()
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
    class EulRollRegister extends Register
    {
        EulRollRegister(RegisterManager regManager) { super(regManager, 0x1C, 2, false); }

        int value;


        public EulRollRegister read()
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
    class EulPitchRegister extends Register
    {
        EulPitchRegister(RegisterManager regManager) { super(regManager, 0x1E, 2, false); }

        int value;


        public EulPitchRegister read()
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
    class QuaWRegister extends Register
    {
        QuaWRegister(RegisterManager regManager) { super(regManager, 0x20, 2, false); }

        int value;


        public QuaWRegister read()
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
    class QuaXRegister extends Register
    {
        QuaXRegister(RegisterManager regManager) { super(regManager, 0x22, 2, false); }

        int value;


        public QuaXRegister read()
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
    class QuaYRegister extends Register
    {
        QuaYRegister(RegisterManager regManager) { super(regManager, 0x24, 2, false); }

        int value;


        public QuaYRegister read()
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
    class QuaZRegister extends Register
    {
        QuaZRegister(RegisterManager regManager) { super(regManager, 0x26, 2, false); }

        int value;


        public QuaZRegister read()
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
    class LinXRegister extends Register
    {
        LinXRegister(RegisterManager regManager) { super(regManager, 0x28, 2, false); }

        int value;


        public LinXRegister read()
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
    class LinYRegister extends Register
    {
        LinYRegister(RegisterManager regManager) { super(regManager, 0x2A, 2, false); }

        int value;


        public LinYRegister read()
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
    class LinZRegister extends Register
    {
        LinZRegister(RegisterManager regManager) { super(regManager, 0x2C, 2, false); }

        int value;


        public LinZRegister read()
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
    class GravXRegister extends Register
    {
        GravXRegister(RegisterManager regManager) { super(regManager, 0x2E, 2, false); }

        int value;


        public GravXRegister read()
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
    class GravYRegister extends Register
    {
        GravYRegister(RegisterManager regManager) { super(regManager, 0x30, 2, false); }

        int value;


        public GravYRegister read()
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
    class GravZRegister extends Register
    {
        GravZRegister(RegisterManager regManager) { super(regManager, 0x32, 2, false); }

        int value;


        public GravZRegister read()
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
    class TempRegister extends Register
    {
        TempRegister(RegisterManager regManager) { super(regManager, 0x34, 1, false); }

        int value;


        public TempRegister read()
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
    class CalibStatRegister extends Register
    {
        CalibStatRegister(RegisterManager regManager) { super(regManager, 0x35, 1, false); }

        int magCalibStatus;
        int accelCalibStatus;
        int gyroCalibStatus;
        int sysCalibStatus;


        public CalibStatRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((magCalibStatus & 0x3) << 0) | ((accelCalibStatus & 0x3) << 2) | ((gyroCalibStatus & 0x3) << 4) | ((sysCalibStatus & 0x3) << 6); }
        public void setValue(long _value)
        {
            magCalibStatus = (int)((_value >> 0) & 0x3);
            accelCalibStatus = (int)((_value >> 2) & 0x3);
            gyroCalibStatus = (int)((_value >> 4) & 0x3);
            sysCalibStatus = (int)((_value >> 6) & 0x3);
        }
    }
    class SelfTestResultRegister extends Register
    {
        SelfTestResultRegister(RegisterManager regManager) { super(regManager, 0x36, 1, false); }

        int accel;
        int mag;
        int gyro;
        int mcu;


        public SelfTestResultRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((accel & 0x1) << 0) | ((mag & 0x1) << 1) | ((gyro & 0x1) << 2) | ((mcu & 0x1) << 3); }
        public void setValue(long _value)
        {
            accel = (int)((_value >> 0) & 0x1);
            mag = (int)((_value >> 1) & 0x1);
            gyro = (int)((_value >> 2) & 0x1);
            mcu = (int)((_value >> 3) & 0x1);
        }
    }
    class InterruptStatusRegister extends Register
    {
        InterruptStatusRegister(RegisterManager regManager) { super(regManager, 0x37, 1, false); }

        int gyroAnyMotion;
        int gyroHighRate;
        int accelHighG;
        int accelAnyMotion;
        int accelNoMotion;


        public InterruptStatusRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((gyroAnyMotion & 0x1) << 2) | ((gyroHighRate & 0x1) << 3) | ((accelHighG & 0x1) << 5) | ((accelAnyMotion & 0x1) << 6) | ((accelNoMotion & 0x1) << 7); }
        public void setValue(long _value)
        {
            gyroAnyMotion = (int)((_value >> 2) & 0x1);
            gyroHighRate = (int)((_value >> 3) & 0x1);
            accelHighG = (int)((_value >> 5) & 0x1);
            accelAnyMotion = (int)((_value >> 6) & 0x1);
            accelNoMotion = (int)((_value >> 7) & 0x1);
        }
    }
    class SysClockStatusRegister extends Register
    {
        SysClockStatusRegister(RegisterManager regManager) { super(regManager, 0x38, 1, false); }

        int mainClock;


        public SysClockStatusRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((mainClock & 0x1) << 0); }
        public void setValue(long _value)
        {
            mainClock = (int)((_value >> 0) & 0x1);
        }
    }
    class SysStatusRegister extends Register
    {
        SysStatusRegister(RegisterManager regManager) { super(regManager, 0x39, 1, false); }

        int value;


        public SysStatusRegister read()
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
    class SysErrRegister extends Register
    {
        SysErrRegister(RegisterManager regManager) { super(regManager, 0x3a, 1, false); }

        int value;


        public SysErrRegister read()
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
    class UnitSelRegister extends Register
    {
        UnitSelRegister(RegisterManager regManager) { super(regManager, 0x3b, 1, false); }

        int accel;
        int gyro;
        int eular;
        int temp;
        int orientationMode;


        public UnitSelRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((accel & 0x1) << 0) | ((gyro & 0x1) << 1) | ((eular & 0x1) << 2) | ((temp & 0x1) << 4) | ((orientationMode & 0x1) << 7); }
        public void setValue(long _value)
        {
            accel = (int)((_value >> 0) & 0x1);
            gyro = (int)((_value >> 1) & 0x1);
            eular = (int)((_value >> 2) & 0x1);
            temp = (int)((_value >> 4) & 0x1);
            orientationMode = (int)((_value >> 7) & 0x1);
        }
    }
    class OperatingModeRegister extends Register
    {
        OperatingModeRegister(RegisterManager regManager) { super(regManager, 0x3d, 1, false); }

        int operatingMode;

                public OperatingModes getOperatingMode() { for (OperatingModes b : OperatingModes.values()) { if(b.getVal() == operatingMode) return b; } return OperatingModes.values()[0]; }
                public void setOperatingMode(OperatingModes enumVal) { operatingMode = enumVal.getVal(); }

        public OperatingModeRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((operatingMode & 0xF) << 0); }
        public void setValue(long _value)
        {
            operatingMode = (int)((_value >> 0) & 0xF);
        }
    }
    class PowerModeRegister extends Register
    {
        PowerModeRegister(RegisterManager regManager) { super(regManager, 0x3e, 1, false); }

        int powerMode;

                public PowerModes getPowerMode() { for (PowerModes b : PowerModes.values()) { if(b.getVal() == powerMode) return b; } return PowerModes.values()[0]; }
                public void setPowerMode(PowerModes enumVal) { powerMode = enumVal.getVal(); }

        public PowerModeRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((powerMode & 0x3) << 0); }
        public void setValue(long _value)
        {
            powerMode = (int)((_value >> 0) & 0x3);
        }
    }
    class SysTriggerRegister extends Register
    {
        SysTriggerRegister(RegisterManager regManager) { super(regManager, 0x3f, 1, false); }

        int selfTest;
        int resetSys;
        int resetInt;
        int clockSel;


        public SysTriggerRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((selfTest & 0x1) << 0) | ((resetSys & 0x1) << 5) | ((resetInt & 0x1) << 6) | ((clockSel & 0x1) << 7); }
        public void setValue(long _value)
        {
            selfTest = (int)((_value >> 0) & 0x1);
            resetSys = (int)((_value >> 5) & 0x1);
            resetInt = (int)((_value >> 6) & 0x1);
            clockSel = (int)((_value >> 7) & 0x1);
        }
    }
    class TempSourceRegister extends Register
    {
        TempSourceRegister(RegisterManager regManager) { super(regManager, 0x40, 1, false); }

        int source;


        public TempSourceRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((source & 0x3) << 0); }
        public void setValue(long _value)
        {
            source = (int)((_value >> 0) & 0x3);
        }
    }
    class AxisMapConfigRegister extends Register
    {
        AxisMapConfigRegister(RegisterManager regManager) { super(regManager, 0x41, 1, false); }

        int x;
        int y;
        int z;


        public AxisMapConfigRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((x & 0x3) << 0) | ((y & 0x3) << 2) | ((z & 0x3) << 4); }
        public void setValue(long _value)
        {
            x = (int)((_value >> 0) & 0x3);
            y = (int)((_value >> 2) & 0x3);
            z = (int)((_value >> 4) & 0x3);
        }
    }
    class AxisMapSignRegister extends Register
    {
        AxisMapSignRegister(RegisterManager regManager) { super(regManager, 0x41, 1, false); }

        int x;
        int y;
        int z;


        public AxisMapSignRegister read()
        {
            manager.read(this);
            return this;
        }

        public long getValue() { return ((x & 0x1) << 0) | ((y & 0x1) << 1) | ((z & 0x1) << 2); }
        public void setValue(long _value)
        {
            x = (int)((_value >> 0) & 0x1);
            y = (int)((_value >> 1) & 0x1);
            z = (int)((_value >> 2) & 0x1);
        }
    }
    class AccelOffsetXRegister extends Register
    {
        AccelOffsetXRegister(RegisterManager regManager) { super(regManager, 0x55, 2, false); }

        int value;


        public AccelOffsetXRegister read()
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
    class AccelOffsetYRegister extends Register
    {
        AccelOffsetYRegister(RegisterManager regManager) { super(regManager, 0x57, 2, false); }

        int value;


        public AccelOffsetYRegister read()
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
    class AccelOffsetZRegister extends Register
    {
        AccelOffsetZRegister(RegisterManager regManager) { super(regManager, 0x59, 2, false); }

        int value;


        public AccelOffsetZRegister read()
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
    class MagnetometerOffsetXRegister extends Register
    {
        MagnetometerOffsetXRegister(RegisterManager regManager) { super(regManager, 0x5B, 2, false); }

        int value;


        public MagnetometerOffsetXRegister read()
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
    class MagnetometerOffsetYRegister extends Register
    {
        MagnetometerOffsetYRegister(RegisterManager regManager) { super(regManager, 0x5D, 2, false); }

        int value;


        public MagnetometerOffsetYRegister read()
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
    class MagnetometerOffsetZRegister extends Register
    {
        MagnetometerOffsetZRegister(RegisterManager regManager) { super(regManager, 0x5F, 2, false); }

        int value;


        public MagnetometerOffsetZRegister read()
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
    class GyroOffsetXRegister extends Register
    {
        GyroOffsetXRegister(RegisterManager regManager) { super(regManager, 0x61, 2, false); }

        int value;


        public GyroOffsetXRegister read()
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
    class GyroOffsetYRegister extends Register
    {
        GyroOffsetYRegister(RegisterManager regManager) { super(regManager, 0x63, 2, false); }

        int value;


        public GyroOffsetYRegister read()
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
    class GyroOffsetZRegister extends Register
    {
        GyroOffsetZRegister(RegisterManager regManager) { super(regManager, 0x65, 2, false); }

        int value;


        public GyroOffsetZRegister read()
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
    class AccelRadiusRegister extends Register
    {
        AccelRadiusRegister(RegisterManager regManager) { super(regManager, 0x67, 2, false); }

        int value;


        public AccelRadiusRegister read()
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
    class MagRadiusRegister extends Register
    {
        MagRadiusRegister(RegisterManager regManager) { super(regManager, 0x69, 2, false); }

        int value;


        public MagRadiusRegister read()
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
