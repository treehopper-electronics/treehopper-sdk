#pragma once
#include "SMBusDevice.h"
#include "Treehopper.Libraries.h"

using namespace Treehopper::Libraries;

namespace Treehopper { namespace Libraries { namespace Sensors { namespace Inertial { 
    class Mpu6050Registers
    {
        private:
            SMBusDevice& _dev;

        public:
            Mpu6050Registers(SMBusDevice& device) : _dev(device)
            {

            }

            class Self_test_x_gyroRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class Self_test_y_gyroRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class Self_test_z_gyroRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class Self_test_x_accelRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class Self_test_y_accelRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class Self_test_z_accelRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class XGyroOffsUsrRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFFFF);
                }
            };

            class YGyroOffsUsrRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFFFF);
                }
            };

            class ZGyroOffsUsrRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFFFF);
                }
            };

            class SampleRateDividerRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class ConfigurationRegister
            {
                public:
                int dlpf;
                int extSyncSet;
                int fifoMode;

                long getValue() { return ((Dlpf & 0x7) << 0) | ((ExtSyncSet & 0x7) << 3) | ((FifoMode & 0x1) << 6); }
                void setValue(long value)
                {
                    dlpf = (int)((value >> 0) & 0x7);
                    extSyncSet = (int)((value >> 3) & 0x7);
                    fifoMode = (int)((value >> 6) & 0x1);
                }
            };

            class GyroConfigRegister
            {
                public:
                int fChoiceBypass;
                int gyroScale;
                int zGyroCten;
                int yGyroCten;

                long getValue() { return ((FChoiceBypass & 0x3) << 0) | ((GyroScale & 0x3) << 3) | ((ZGyroCten & 0x1) << 5) | ((YGyroCten & 0x1) << 6); }
                void setValue(long value)
                {
                    fChoiceBypass = (int)((value >> 0) & 0x3);
                    gyroScale = (int)((value >> 3) & 0x3);
                    zGyroCten = (int)((value >> 5) & 0x1);
                    yGyroCten = (int)((value >> 6) & 0x1);
                }
            };

            class AccelConfigRegister
            {
                public:
                int accelScale;
                int accelZselfTest;
                int accelYselfTest;
                int accelXselfTest;

                long getValue() { return ((AccelScale & 0x3) << 3) | ((AccelZselfTest & 0x1) << 5) | ((AccelYselfTest & 0x1) << 6) | ((AccelXselfTest & 0x1) << 7); }
                void setValue(long value)
                {
                    accelScale = (int)((value >> 3) & 0x3);
                    accelZselfTest = (int)((value >> 5) & 0x1);
                    accelYselfTest = (int)((value >> 6) & 0x1);
                    accelXselfTest = (int)((value >> 7) & 0x1);
                }
            };

            class AccelConfig2Register
            {
                public:
                int dlpfCfg;
                int accelFchoice;

                long getValue() { return ((DlpfCfg & 0x7) << 0) | ((AccelFchoice & 0x1) << 3); }
                void setValue(long value)
                {
                    dlpfCfg = (int)((value >> 0) & 0x7);
                    accelFchoice = (int)((value >> 3) & 0x1);
                }
            };

            class LowPowerAccelerometerOdrControlRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xF);
                }
            };

            class WomThresholdRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class FifoEnableRegister
            {
                public:
                int slv0;
                int slv1;
                int slv2;
                int accel;
                int gyroZout;
                int gyroYout;
                int gyroXout;
                int tempOut;

                long getValue() { return ((Slv0 & 0x1) << 0) | ((Slv1 & 0x1) << 1) | ((Slv2 & 0x1) << 2) | ((Accel & 0x1) << 3) | ((GyroZout & 0x1) << 4) | ((GyroYout & 0x1) << 5) | ((GyroXout & 0x1) << 6) | ((TempOut & 0x1) << 7); }
                void setValue(long value)
                {
                    slv0 = (int)((value >> 0) & 0x1);
                    slv1 = (int)((value >> 1) & 0x1);
                    slv2 = (int)((value >> 2) & 0x1);
                    accel = (int)((value >> 3) & 0x1);
                    gyroZout = (int)((value >> 4) & 0x1);
                    gyroYout = (int)((value >> 5) & 0x1);
                    gyroXout = (int)((value >> 6) & 0x1);
                    tempOut = (int)((value >> 7) & 0x1);
                }
            };

            class I2cMasterControlRegister
            {
                public:
                int i2cMasterClock;
                int i2cMstPnsr;
                int slv3FifoEn;
                int waitForEs;
                int multMstEn;

                long getValue() { return ((I2cMasterClock & 0xF) << 0) | ((I2cMstPnsr & 0x1) << 4) | ((Slv3FifoEn & 0x1) << 5) | ((WaitForEs & 0x1) << 6) | ((MultMstEn & 0x1) << 7); }
                void setValue(long value)
                {
                    i2cMasterClock = (int)((value >> 0) & 0xF);
                    i2cMstPnsr = (int)((value >> 4) & 0x1);
                    slv3FifoEn = (int)((value >> 5) & 0x1);
                    waitForEs = (int)((value >> 6) & 0x1);
                    multMstEn = (int)((value >> 7) & 0x1);
                }
            };

            class I2cSlv0AddrRegister
            {
                public:
                int id;
                int rnw;

                long getValue() { return ((Id & 0x7F) << 0) | ((Rnw & 0x1) << 7); }
                void setValue(long value)
                {
                    id = (int)((value >> 0) & 0x7F);
                    rnw = (int)((value >> 7) & 0x1);
                }
            };

            class I2cSlv0RegRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv0CtrlRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv1AddrRegister
            {
                public:
                int id;
                int rnw;

                long getValue() { return ((Id & 0x7F) << 0) | ((Rnw & 0x1) << 7); }
                void setValue(long value)
                {
                    id = (int)((value >> 0) & 0x7F);
                    rnw = (int)((value >> 7) & 0x1);
                }
            };

            class I2cSlv1RegRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv1CtrlRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv2AddrRegister
            {
                public:
                int id;
                int rnw;

                long getValue() { return ((Id & 0x7F) << 0) | ((Rnw & 0x1) << 7); }
                void setValue(long value)
                {
                    id = (int)((value >> 0) & 0x7F);
                    rnw = (int)((value >> 7) & 0x1);
                }
            };

            class I2cSlv2RegRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv2CtrlRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv3AddrRegister
            {
                public:
                int id;
                int rnw;

                long getValue() { return ((Id & 0x7F) << 0) | ((Rnw & 0x1) << 7); }
                void setValue(long value)
                {
                    id = (int)((value >> 0) & 0x7F);
                    rnw = (int)((value >> 7) & 0x1);
                }
            };

            class I2cSlv3RegRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv3CtrlRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv4AddrRegister
            {
                public:
                int id;
                int rnw;

                long getValue() { return ((Id & 0x7F) << 0) | ((Rnw & 0x1) << 7); }
                void setValue(long value)
                {
                    id = (int)((value >> 0) & 0x7F);
                    rnw = (int)((value >> 7) & 0x1);
                }
            };

            class I2cSlv4RegRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv4DoRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv4CtrlRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv4DiRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cMstStatusRegister
            {
                public:
                int slv0Nack;
                int slv1Nack;
                int slv2Nack;
                int slv3Nack;
                int slv4Nack;
                int lostArb;
                int slv4Done;
                int passThrough;

                long getValue() { return ((Slv0Nack & 0x1) << 0) | ((Slv1Nack & 0x1) << 1) | ((Slv2Nack & 0x1) << 2) | ((Slv3Nack & 0x1) << 3) | ((Slv4Nack & 0x1) << 4) | ((LostArb & 0x1) << 5) | ((Slv4Done & 0x1) << 6) | ((PassThrough & 0x1) << 7); }
                void setValue(long value)
                {
                    slv0Nack = (int)((value >> 0) & 0x1);
                    slv1Nack = (int)((value >> 1) & 0x1);
                    slv2Nack = (int)((value >> 2) & 0x1);
                    slv3Nack = (int)((value >> 3) & 0x1);
                    slv4Nack = (int)((value >> 4) & 0x1);
                    lostArb = (int)((value >> 5) & 0x1);
                    slv4Done = (int)((value >> 6) & 0x1);
                    passThrough = (int)((value >> 7) & 0x1);
                }
            };

            class IntPinCfgRegister
            {
                public:
                int bypassEn;
                int fsyncIntModeEnable;
                int actlFsync;
                int intAnyRd2Clear;
                int latchIntEn;
                int open;
                int actl;

                long getValue() { return ((BypassEn & 0x1) << 1) | ((FsyncIntModeEnable & 0x1) << 2) | ((ActlFsync & 0x1) << 3) | ((IntAnyRd2Clear & 0x1) << 4) | ((LatchIntEn & 0x1) << 5) | ((Open & 0x1) << 6) | ((Actl & 0x1) << 7); }
                void setValue(long value)
                {
                    bypassEn = (int)((value >> 1) & 0x1);
                    fsyncIntModeEnable = (int)((value >> 2) & 0x1);
                    actlFsync = (int)((value >> 3) & 0x1);
                    intAnyRd2Clear = (int)((value >> 4) & 0x1);
                    latchIntEn = (int)((value >> 5) & 0x1);
                    open = (int)((value >> 6) & 0x1);
                    actl = (int)((value >> 7) & 0x1);
                }
            };

            class IntEnableRegister
            {
                public:
                int RawReadyEnable;
                int fsyncIntEnable;
                int fifoIntEnable;
                int fifoOverflowEnable;
                int womEnable;

                long getValue() { return ((RawReadyEnable & 0x1) << 0) | ((FsyncIntEnable & 0x1) << 2) | ((FifoIntEnable & 0x1) << 3) | ((FifoOverflowEnable & 0x1) << 4) | ((WomEnable & 0x1) << 1); }
                void setValue(long value)
                {
                    RawReadyEnable = (int)((value >> 0) & 0x1);
                    fsyncIntEnable = (int)((value >> 2) & 0x1);
                    fifoIntEnable = (int)((value >> 3) & 0x1);
                    fifoOverflowEnable = (int)((value >> 4) & 0x1);
                    womEnable = (int)((value >> 1) & 0x1);
                }
            };

            class IntStatusRegister
            {
                public:
                int rawDataReadyInt;
                int fsyncInt;
                int fifoOverflowInt;
                int womInt;

                long getValue() { return ((RawDataReadyInt & 0x1) << 0) | ((FsyncInt & 0x1) << 2) | ((FifoOverflowInt & 0x1) << 3) | ((WomInt & 0x1) << 1); }
                void setValue(long value)
                {
                    rawDataReadyInt = (int)((value >> 0) & 0x1);
                    fsyncInt = (int)((value >> 2) & 0x1);
                    fifoOverflowInt = (int)((value >> 3) & 0x1);
                    womInt = (int)((value >> 1) & 0x1);
                }
            };

            class Accel_xRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)(((value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }
            };

            class Accel_yRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)(((value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }
            };

            class Accel_zRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)(((value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }
            };

            class TempRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)(((value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }
            };

            class Gyro_xRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)(((value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }
            };

            class Gyro_yRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)(((value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }
            };

            class Gyro_zRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)(((value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }
            };

            class ExtSensDataRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0x0) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0x0);
                }
            };

            class I2cSlv0doRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv1doRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv2doRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv3doRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cMstDelayCtrlRegister
            {
                public:
                int slv0DelayEn;
                int slv1DelayEn;
                int slv2DelayEn;
                int slv3DelayEn;
                int slv4DelayEn;
                int delayEsShadow;

                long getValue() { return ((Slv0DelayEn & 0x1) << 0) | ((Slv1DelayEn & 0x1) << 1) | ((Slv2DelayEn & 0x1) << 2) | ((Slv3DelayEn & 0x1) << 3) | ((Slv4DelayEn & 0x1) << 4) | ((DelayEsShadow & 0x1) << 2); }
                void setValue(long value)
                {
                    slv0DelayEn = (int)((value >> 0) & 0x1);
                    slv1DelayEn = (int)((value >> 1) & 0x1);
                    slv2DelayEn = (int)((value >> 2) & 0x1);
                    slv3DelayEn = (int)((value >> 3) & 0x1);
                    slv4DelayEn = (int)((value >> 4) & 0x1);
                    delayEsShadow = (int)((value >> 2) & 0x1);
                }
            };

            class SignalPathResetRegister
            {
                public:
                int tempReset;
                int accelReset;
                int gyroReset;

                long getValue() { return ((TempReset & 0x1) << 0) | ((AccelReset & 0x1) << 1) | ((GyroReset & 0x1) << 2); }
                void setValue(long value)
                {
                    tempReset = (int)((value >> 0) & 0x1);
                    accelReset = (int)((value >> 1) & 0x1);
                    gyroReset = (int)((value >> 2) & 0x1);
                }
            };

            class AccelIntCtrlRegister
            {
                public:
                int accelIntelMode;
                int accelIntelEnable;

                long getValue() { return ((AccelIntelMode & 0x1) << 6) | ((AccelIntelEnable & 0x1) << 7); }
                void setValue(long value)
                {
                    accelIntelMode = (int)((value >> 6) & 0x1);
                    accelIntelEnable = (int)((value >> 7) & 0x1);
                }
            };

            class UserCtrlRegister
            {
                public:
                int sigConditionReset;
                int i2cMasterReset;
                int fifoReset;
                int i2cIfDisable;
                int i2cMasterEnable;
                int fifoEnable;

                long getValue() { return ((SigConditionReset & 0x1) << 0) | ((I2cMasterReset & 0x1) << 1) | ((FifoReset & 0x1) << 2) | ((I2cIfDisable & 0x1) << 1) | ((I2cMasterEnable & 0x1) << 2) | ((FifoEnable & 0x1) << 3); }
                void setValue(long value)
                {
                    sigConditionReset = (int)((value >> 0) & 0x1);
                    i2cMasterReset = (int)((value >> 1) & 0x1);
                    fifoReset = (int)((value >> 2) & 0x1);
                    i2cIfDisable = (int)((value >> 1) & 0x1);
                    i2cMasterEnable = (int)((value >> 2) & 0x1);
                    fifoEnable = (int)((value >> 3) & 0x1);
                }
            };

            class PowerMgmt1Register
            {
                public:
                int clockSel;
                int powerDownPtat;
                int gyroStandby;
                int cycle;
                int sleep;
                int reset;

                long getValue() { return ((ClockSel & 0x7) << 0) | ((PowerDownPtat & 0x1) << 3) | ((GyroStandby & 0x1) << 4) | ((Cycle & 0x1) << 5) | ((Sleep & 0x1) << 6) | ((Reset & 0x1) << 7); }
                void setValue(long value)
                {
                    clockSel = (int)((value >> 0) & 0x7);
                    powerDownPtat = (int)((value >> 3) & 0x1);
                    gyroStandby = (int)((value >> 4) & 0x1);
                    cycle = (int)((value >> 5) & 0x1);
                    sleep = (int)((value >> 6) & 0x1);
                    reset = (int)((value >> 7) & 0x1);
                }
            };

            class PowerMgmt2Register
            {
                public:
                int disableZG;
                int disableYG;
                int disableXG;
                int disableZA;
                int disableYA;
                int disableXA;

                long getValue() { return ((DisableZG & 0x1) << 0) | ((DisableYG & 0x1) << 1) | ((DisableXG & 0x1) << 2) | ((DisableZA & 0x1) << 3) | ((DisableYA & 0x1) << 4) | ((DisableXA & 0x1) << 5); }
                void setValue(long value)
                {
                    disableZG = (int)((value >> 0) & 0x1);
                    disableYG = (int)((value >> 1) & 0x1);
                    disableXG = (int)((value >> 2) & 0x1);
                    disableZA = (int)((value >> 3) & 0x1);
                    disableYA = (int)((value >> 4) & 0x1);
                    disableXA = (int)((value >> 5) & 0x1);
                }
            };

            class FifoCountRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0x1FFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0x1FFF);
                }
            };

            class FifoRWRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class WhoAmIRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class XAccelOffsetRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0x7FFF) << 1); }
                void setValue(long value)
                {
                    value = (int)((value >> 1) & 0x7FFF);
                }
            };

            class YAccelOffsetRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0x7FFF) << 1); }
                void setValue(long value)
                {
                    value = (int)((value >> 1) & 0x7FFF);
                }
            };

            class ZAccelOffsetRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0x7FFF) << 1); }
                void setValue(long value)
                {
                    value = (int)((value >> 1) & 0x7FFF);
                }
            };

            Self_test_x_gyroRegister self_test_x_gyro;
            Self_test_y_gyroRegister self_test_y_gyro;
            Self_test_z_gyroRegister self_test_z_gyro;
            Self_test_x_accelRegister self_test_x_accel;
            Self_test_y_accelRegister self_test_y_accel;
            Self_test_z_accelRegister self_test_z_accel;
            XGyroOffsUsrRegister xGyroOffsUsr;
            YGyroOffsUsrRegister yGyroOffsUsr;
            ZGyroOffsUsrRegister zGyroOffsUsr;
            SampleRateDividerRegister sampleRateDivider;
            ConfigurationRegister configuration;
            GyroConfigRegister gyroConfig;
            AccelConfigRegister accelConfig;
            AccelConfig2Register accelConfig2;
            LowPowerAccelerometerOdrControlRegister lowPowerAccelerometerOdrControl;
            WomThresholdRegister womThreshold;
            FifoEnableRegister fifoEnable;
            I2cMasterControlRegister i2cMasterControl;
            I2cSlv0AddrRegister i2cSlv0Addr;
            I2cSlv0RegRegister i2cSlv0Reg;
            I2cSlv0CtrlRegister i2cSlv0Ctrl;
            I2cSlv1AddrRegister i2cSlv1Addr;
            I2cSlv1RegRegister i2cSlv1Reg;
            I2cSlv1CtrlRegister i2cSlv1Ctrl;
            I2cSlv2AddrRegister i2cSlv2Addr;
            I2cSlv2RegRegister i2cSlv2Reg;
            I2cSlv2CtrlRegister i2cSlv2Ctrl;
            I2cSlv3AddrRegister i2cSlv3Addr;
            I2cSlv3RegRegister i2cSlv3Reg;
            I2cSlv3CtrlRegister i2cSlv3Ctrl;
            I2cSlv4AddrRegister i2cSlv4Addr;
            I2cSlv4RegRegister i2cSlv4Reg;
            I2cSlv4DoRegister i2cSlv4Do;
            I2cSlv4CtrlRegister i2cSlv4Ctrl;
            I2cSlv4DiRegister i2cSlv4Di;
            I2cMstStatusRegister i2cMstStatus;
            IntPinCfgRegister intPinCfg;
            IntEnableRegister intEnable;
            IntStatusRegister intStatus;
            Accel_xRegister accel_x;
            Accel_yRegister accel_y;
            Accel_zRegister accel_z;
            TempRegister temp;
            Gyro_xRegister gyro_x;
            Gyro_yRegister gyro_y;
            Gyro_zRegister gyro_z;
            ExtSensDataRegister extSensData;
            I2cSlv0doRegister i2cSlv0do;
            I2cSlv1doRegister i2cSlv1do;
            I2cSlv2doRegister i2cSlv2do;
            I2cSlv3doRegister i2cSlv3do;
            I2cMstDelayCtrlRegister i2cMstDelayCtrl;
            SignalPathResetRegister signalPathReset;
            AccelIntCtrlRegister accelIntCtrl;
            UserCtrlRegister userCtrl;
            PowerMgmt1Register powerMgmt1;
            PowerMgmt2Register powerMgmt2;
            FifoCountRegister fifoCount;
            FifoRWRegister fifoRW;
            WhoAmIRegister whoAmI;
            XAccelOffsetRegister xAccelOffset;
            YAccelOffsetRegister yAccelOffset;
            ZAccelOffsetRegister zAccelOffset;

        void getBytes(long val, int width, bool isLittleEndian, uint8_t* output)
        {
            for (int i = 0; i < width; i++) 
                output[i] = (uint8_t) ((val >> (8 * i)) & 0xFF);

            // TODO: Fix endian
            // if (BitConverter.IsLittleEndian ^ isLittleEndian) 
            //         bytes = bytes.Reverse().ToArray(); 
        }

        long getValue(uint8_t* bytes, int count, bool isLittleEndian)
        {
            // TODO: Fix endian
            // if (BitConverter.IsLittleEndian ^ isLittleEndian) 
            //         bytes = bytes.Reverse().ToArray(); 
 
            long regVal = 0; 
 
            for (int i = 0; i < count; i++) 
                    regVal |= bytes[i] << (i * 8);

            return regVal;
        }

        void flush()
        {
            uint8_t bytes[8];
        }

        void update()
        {
            uint8_t bytes[14];
            int i = 0;
            _dev.readBufferData(59, bytes, 14);
            accel_x.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            accel_y.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            accel_z.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            temp.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            gyro_x.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            gyro_y.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            gyro_z.setValue(getValue(&bytes[i], 2, ));
            i += 2;
        }

    };
 }  }  } }