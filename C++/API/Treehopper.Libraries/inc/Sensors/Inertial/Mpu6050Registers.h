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
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class Self_test_y_gyroRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class Self_test_z_gyroRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class Self_test_x_accelRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class Self_test_y_accelRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class Self_test_z_accelRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class XGyroOffsUsrRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFFFF);
                }
            };

            class YGyroOffsUsrRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFFFF);
                }
            };

            class ZGyroOffsUsrRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFFFF);
                }
            };

            class SampleRateDividerRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class ConfigurationRegister
            {
                public:
                int Dlpf;
                int ExtSyncSet;
                int FifoMode;

                long getValue() { return ((Dlpf & 0x7) << 0) | ((ExtSyncSet & 0x7) << 3) | ((FifoMode & 0x1) << 6); }
                void setValue(long value)
                {
                    Dlpf = (int)((value >> 0) & 0x7);
                    ExtSyncSet = (int)((value >> 3) & 0x7);
                    FifoMode = (int)((value >> 6) & 0x1);
                }
            };

            class GyroConfigRegister
            {
                public:
                int FChoiceBypass;
                int GyroFsSel;
                int ZGyroCten;
                int YGyroCten;

                long getValue() { return ((FChoiceBypass & 0x3) << 0) | ((GyroFsSel & 0x3) << 3) | ((ZGyroCten & 0x1) << 5) | ((YGyroCten & 0x1) << 6); }
                void setValue(long value)
                {
                    FChoiceBypass = (int)((value >> 0) & 0x3);
                    GyroFsSel = (int)((value >> 3) & 0x3);
                    ZGyroCten = (int)((value >> 5) & 0x1);
                    YGyroCten = (int)((value >> 6) & 0x1);
                }
            };

            class AccelConfigRegister
            {
                public:
                int AccelFsSel;
                int AccelZselfTest;
                int AccelYselfTest;
                int AccelXselfTest;

                long getValue() { return ((AccelFsSel & 0x3) << 3) | ((AccelZselfTest & 0x1) << 5) | ((AccelYselfTest & 0x1) << 6) | ((AccelXselfTest & 0x1) << 7); }
                void setValue(long value)
                {
                    AccelFsSel = (int)((value >> 3) & 0x3);
                    AccelZselfTest = (int)((value >> 5) & 0x1);
                    AccelYselfTest = (int)((value >> 6) & 0x1);
                    AccelXselfTest = (int)((value >> 7) & 0x1);
                }
            };

            class AccelConfig2Register
            {
                public:
                int DlpfCfg;
                int AccelFchoice;

                long getValue() { return ((DlpfCfg & 0x7) << 0) | ((AccelFchoice & 0x1) << 3); }
                void setValue(long value)
                {
                    DlpfCfg = (int)((value >> 0) & 0x7);
                    AccelFchoice = (int)((value >> 3) & 0x1);
                }
            };

            class LowPowerAccelerometerOdrControlRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xF);
                }
            };

            class WomThresholdRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class FifoEnableRegister
            {
                public:
                int Slv0;
                int Slv1;
                int Slv2;
                int Accel;
                int GyroZout;
                int GyroYout;
                int GyroXout;
                int TempOut;

                long getValue() { return ((Slv0 & 0x1) << 0) | ((Slv1 & 0x1) << 1) | ((Slv2 & 0x1) << 2) | ((Accel & 0x1) << 3) | ((GyroZout & 0x1) << 4) | ((GyroYout & 0x1) << 5) | ((GyroXout & 0x1) << 6) | ((TempOut & 0x1) << 7); }
                void setValue(long value)
                {
                    Slv0 = (int)((value >> 0) & 0x1);
                    Slv1 = (int)((value >> 1) & 0x1);
                    Slv2 = (int)((value >> 2) & 0x1);
                    Accel = (int)((value >> 3) & 0x1);
                    GyroZout = (int)((value >> 4) & 0x1);
                    GyroYout = (int)((value >> 5) & 0x1);
                    GyroXout = (int)((value >> 6) & 0x1);
                    TempOut = (int)((value >> 7) & 0x1);
                }
            };

            class I2cMasterControlRegister
            {
                public:
                int I2cMasterClock;
                int I2cMstPnsr;
                int Slv3FifoEn;
                int WaitForEs;
                int MultMstEn;

                long getValue() { return ((I2cMasterClock & 0xF) << 0) | ((I2cMstPnsr & 0x1) << 4) | ((Slv3FifoEn & 0x1) << 5) | ((WaitForEs & 0x1) << 6) | ((MultMstEn & 0x1) << 7); }
                void setValue(long value)
                {
                    I2cMasterClock = (int)((value >> 0) & 0xF);
                    I2cMstPnsr = (int)((value >> 4) & 0x1);
                    Slv3FifoEn = (int)((value >> 5) & 0x1);
                    WaitForEs = (int)((value >> 6) & 0x1);
                    MultMstEn = (int)((value >> 7) & 0x1);
                }
            };

            class I2cSlv0AddrRegister
            {
                public:
                int Id;
                int Rnw;

                long getValue() { return ((Id & 0x7F) << 0) | ((Rnw & 0x1) << 7); }
                void setValue(long value)
                {
                    Id = (int)((value >> 0) & 0x7F);
                    Rnw = (int)((value >> 7) & 0x1);
                }
            };

            class I2cSlv0RegRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv0CtrlRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv1AddrRegister
            {
                public:
                int Id;
                int Rnw;

                long getValue() { return ((Id & 0x7F) << 0) | ((Rnw & 0x1) << 7); }
                void setValue(long value)
                {
                    Id = (int)((value >> 0) & 0x7F);
                    Rnw = (int)((value >> 7) & 0x1);
                }
            };

            class I2cSlv1RegRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv1CtrlRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv2AddrRegister
            {
                public:
                int Id;
                int Rnw;

                long getValue() { return ((Id & 0x7F) << 0) | ((Rnw & 0x1) << 7); }
                void setValue(long value)
                {
                    Id = (int)((value >> 0) & 0x7F);
                    Rnw = (int)((value >> 7) & 0x1);
                }
            };

            class I2cSlv2RegRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv2CtrlRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv3AddrRegister
            {
                public:
                int Id;
                int Rnw;

                long getValue() { return ((Id & 0x7F) << 0) | ((Rnw & 0x1) << 7); }
                void setValue(long value)
                {
                    Id = (int)((value >> 0) & 0x7F);
                    Rnw = (int)((value >> 7) & 0x1);
                }
            };

            class I2cSlv3RegRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv3CtrlRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv4AddrRegister
            {
                public:
                int Id;
                int Rnw;

                long getValue() { return ((Id & 0x7F) << 0) | ((Rnw & 0x1) << 7); }
                void setValue(long value)
                {
                    Id = (int)((value >> 0) & 0x7F);
                    Rnw = (int)((value >> 7) & 0x1);
                }
            };

            class I2cSlv4RegRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv4DoRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv4CtrlRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv4DiRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cMstStatusRegister
            {
                public:
                int Slv0Nack;
                int Slv1Nack;
                int Slv2Nack;
                int Slv3Nack;
                int Slv4Nack;
                int LostArb;
                int Slv4Done;
                int PassThrough;

                long getValue() { return ((Slv0Nack & 0x1) << 0) | ((Slv1Nack & 0x1) << 1) | ((Slv2Nack & 0x1) << 2) | ((Slv3Nack & 0x1) << 3) | ((Slv4Nack & 0x1) << 4) | ((LostArb & 0x1) << 5) | ((Slv4Done & 0x1) << 6) | ((PassThrough & 0x1) << 7); }
                void setValue(long value)
                {
                    Slv0Nack = (int)((value >> 0) & 0x1);
                    Slv1Nack = (int)((value >> 1) & 0x1);
                    Slv2Nack = (int)((value >> 2) & 0x1);
                    Slv3Nack = (int)((value >> 3) & 0x1);
                    Slv4Nack = (int)((value >> 4) & 0x1);
                    LostArb = (int)((value >> 5) & 0x1);
                    Slv4Done = (int)((value >> 6) & 0x1);
                    PassThrough = (int)((value >> 7) & 0x1);
                }
            };

            class IntPinCfgRegister
            {
                public:
                int BypassEn;
                int FsyncIntModeEnable;
                int ActlFsync;
                int IntAnyRd2Clear;
                int LatchIntEn;
                int Open;
                int Actl;

                long getValue() { return ((BypassEn & 0x1) << 1) | ((FsyncIntModeEnable & 0x1) << 2) | ((ActlFsync & 0x1) << 3) | ((IntAnyRd2Clear & 0x1) << 4) | ((LatchIntEn & 0x1) << 5) | ((Open & 0x1) << 6) | ((Actl & 0x1) << 7); }
                void setValue(long value)
                {
                    BypassEn = (int)((value >> 1) & 0x1);
                    FsyncIntModeEnable = (int)((value >> 2) & 0x1);
                    ActlFsync = (int)((value >> 3) & 0x1);
                    IntAnyRd2Clear = (int)((value >> 4) & 0x1);
                    LatchIntEn = (int)((value >> 5) & 0x1);
                    Open = (int)((value >> 6) & 0x1);
                    Actl = (int)((value >> 7) & 0x1);
                }
            };

            class IntEnableRegister
            {
                public:
                int RawReadyEnable;
                int FsyncIntEnable;
                int FifoIntEnable;
                int FifoOverflowEnable;
                int WomEnable;

                long getValue() { return ((RawReadyEnable & 0x1) << 0) | ((FsyncIntEnable & 0x1) << 2) | ((FifoIntEnable & 0x1) << 3) | ((FifoOverflowEnable & 0x1) << 4) | ((WomEnable & 0x1) << 1); }
                void setValue(long value)
                {
                    RawReadyEnable = (int)((value >> 0) & 0x1);
                    FsyncIntEnable = (int)((value >> 2) & 0x1);
                    FifoIntEnable = (int)((value >> 3) & 0x1);
                    FifoOverflowEnable = (int)((value >> 4) & 0x1);
                    WomEnable = (int)((value >> 1) & 0x1);
                }
            };

            class IntStatusRegister
            {
                public:
                int RawDataReadyInt;
                int FsyncInt;
                int FifoOverflowInt;
                int WomInt;

                long getValue() { return ((RawDataReadyInt & 0x1) << 0) | ((FsyncInt & 0x1) << 2) | ((FifoOverflowInt & 0x1) << 3) | ((WomInt & 0x1) << 1); }
                void setValue(long value)
                {
                    RawDataReadyInt = (int)((value >> 0) & 0x1);
                    FsyncInt = (int)((value >> 2) & 0x1);
                    FifoOverflowInt = (int)((value >> 3) & 0x1);
                    WomInt = (int)((value >> 1) & 0x1);
                }
            };

            class Accel_xRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)(((value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }
            };

            class Accel_yRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)(((value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }
            };

            class Accel_zRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)(((value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }
            };

            class TempRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)(((value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }
            };

            class Gyro_xRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)(((value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }
            };

            class Gyro_yRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)(((value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }
            };

            class Gyro_zRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)(((value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }
            };

            class ExtSensDataRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0x0) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0x0);
                }
            };

            class I2cSlv0doRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv1doRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv2doRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cSlv3doRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class I2cMstDelayCtrlRegister
            {
                public:
                int Slv0DelayEn;
                int Slv1DelayEn;
                int Slv2DelayEn;
                int Slv3DelayEn;
                int Slv4DelayEn;
                int DelayEsShadow;

                long getValue() { return ((Slv0DelayEn & 0x1) << 0) | ((Slv1DelayEn & 0x1) << 1) | ((Slv2DelayEn & 0x1) << 2) | ((Slv3DelayEn & 0x1) << 3) | ((Slv4DelayEn & 0x1) << 4) | ((DelayEsShadow & 0x1) << 2); }
                void setValue(long value)
                {
                    Slv0DelayEn = (int)((value >> 0) & 0x1);
                    Slv1DelayEn = (int)((value >> 1) & 0x1);
                    Slv2DelayEn = (int)((value >> 2) & 0x1);
                    Slv3DelayEn = (int)((value >> 3) & 0x1);
                    Slv4DelayEn = (int)((value >> 4) & 0x1);
                    DelayEsShadow = (int)((value >> 2) & 0x1);
                }
            };

            class SignalPathResetRegister
            {
                public:
                int TempReset;
                int AccelReset;
                int GyroReset;

                long getValue() { return ((TempReset & 0x1) << 0) | ((AccelReset & 0x1) << 1) | ((GyroReset & 0x1) << 2); }
                void setValue(long value)
                {
                    TempReset = (int)((value >> 0) & 0x1);
                    AccelReset = (int)((value >> 1) & 0x1);
                    GyroReset = (int)((value >> 2) & 0x1);
                }
            };

            class AccelIntCtrlRegister
            {
                public:
                int AccelIntelMode;
                int AccelIntelEnable;

                long getValue() { return ((AccelIntelMode & 0x1) << 6) | ((AccelIntelEnable & 0x1) << 7); }
                void setValue(long value)
                {
                    AccelIntelMode = (int)((value >> 6) & 0x1);
                    AccelIntelEnable = (int)((value >> 7) & 0x1);
                }
            };

            class UserCtrlRegister
            {
                public:
                int SigConditionReset;
                int I2cMasterReset;
                int FifoReset;
                int I2cIfDisable;
                int I2cMasterEnable;
                int FifoEnable;

                long getValue() { return ((SigConditionReset & 0x1) << 0) | ((I2cMasterReset & 0x1) << 1) | ((FifoReset & 0x1) << 2) | ((I2cIfDisable & 0x1) << 1) | ((I2cMasterEnable & 0x1) << 2) | ((FifoEnable & 0x1) << 3); }
                void setValue(long value)
                {
                    SigConditionReset = (int)((value >> 0) & 0x1);
                    I2cMasterReset = (int)((value >> 1) & 0x1);
                    FifoReset = (int)((value >> 2) & 0x1);
                    I2cIfDisable = (int)((value >> 1) & 0x1);
                    I2cMasterEnable = (int)((value >> 2) & 0x1);
                    FifoEnable = (int)((value >> 3) & 0x1);
                }
            };

            class PowerMgmt1Register
            {
                public:
                int ClockSel;
                int PowerDownPtat;
                int GyroStandby;
                int Cycle;
                int Sleep;
                int Reset;

                long getValue() { return ((ClockSel & 0x7) << 0) | ((PowerDownPtat & 0x1) << 3) | ((GyroStandby & 0x1) << 4) | ((Cycle & 0x1) << 5) | ((Sleep & 0x1) << 6) | ((Reset & 0x1) << 7); }
                void setValue(long value)
                {
                    ClockSel = (int)((value >> 0) & 0x7);
                    PowerDownPtat = (int)((value >> 3) & 0x1);
                    GyroStandby = (int)((value >> 4) & 0x1);
                    Cycle = (int)((value >> 5) & 0x1);
                    Sleep = (int)((value >> 6) & 0x1);
                    Reset = (int)((value >> 7) & 0x1);
                }
            };

            class PowerMgmt2Register
            {
                public:
                int DisableZG;
                int DisableYG;
                int DisableXG;
                int DisableZA;
                int DisableYA;
                int DisableXA;

                long getValue() { return ((DisableZG & 0x1) << 0) | ((DisableYG & 0x1) << 1) | ((DisableXG & 0x1) << 2) | ((DisableZA & 0x1) << 3) | ((DisableYA & 0x1) << 4) | ((DisableXA & 0x1) << 5); }
                void setValue(long value)
                {
                    DisableZG = (int)((value >> 0) & 0x1);
                    DisableYG = (int)((value >> 1) & 0x1);
                    DisableXG = (int)((value >> 2) & 0x1);
                    DisableZA = (int)((value >> 3) & 0x1);
                    DisableYA = (int)((value >> 4) & 0x1);
                    DisableXA = (int)((value >> 5) & 0x1);
                }
            };

            class FifoCountRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0x1FFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0x1FFF);
                }
            };

            class FifoRWRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class WhoAmIRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class XAccelOffsetRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0x7FFF) << 1); }
                void setValue(long value)
                {
                    Value = (int)((value >> 1) & 0x7FFF);
                }
            };

            class YAccelOffsetRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0x7FFF) << 1); }
                void setValue(long value)
                {
                    Value = (int)((value >> 1) & 0x7FFF);
                }
            };

            class ZAccelOffsetRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0x7FFF) << 1); }
                void setValue(long value)
                {
                    Value = (int)((value >> 1) & 0x7FFF);
                }
            };

            Self_test_x_gyroRegister Self_test_x_gyro;
            Self_test_y_gyroRegister Self_test_y_gyro;
            Self_test_z_gyroRegister Self_test_z_gyro;
            Self_test_x_accelRegister Self_test_x_accel;
            Self_test_y_accelRegister Self_test_y_accel;
            Self_test_z_accelRegister Self_test_z_accel;
            XGyroOffsUsrRegister XGyroOffsUsr;
            YGyroOffsUsrRegister YGyroOffsUsr;
            ZGyroOffsUsrRegister ZGyroOffsUsr;
            SampleRateDividerRegister SampleRateDivider;
            ConfigurationRegister Configuration;
            GyroConfigRegister GyroConfig;
            AccelConfigRegister AccelConfig;
            AccelConfig2Register AccelConfig2;
            LowPowerAccelerometerOdrControlRegister LowPowerAccelerometerOdrControl;
            WomThresholdRegister WomThreshold;
            FifoEnableRegister FifoEnable;
            I2cMasterControlRegister I2cMasterControl;
            I2cSlv0AddrRegister I2cSlv0Addr;
            I2cSlv0RegRegister I2cSlv0Reg;
            I2cSlv0CtrlRegister I2cSlv0Ctrl;
            I2cSlv1AddrRegister I2cSlv1Addr;
            I2cSlv1RegRegister I2cSlv1Reg;
            I2cSlv1CtrlRegister I2cSlv1Ctrl;
            I2cSlv2AddrRegister I2cSlv2Addr;
            I2cSlv2RegRegister I2cSlv2Reg;
            I2cSlv2CtrlRegister I2cSlv2Ctrl;
            I2cSlv3AddrRegister I2cSlv3Addr;
            I2cSlv3RegRegister I2cSlv3Reg;
            I2cSlv3CtrlRegister I2cSlv3Ctrl;
            I2cSlv4AddrRegister I2cSlv4Addr;
            I2cSlv4RegRegister I2cSlv4Reg;
            I2cSlv4DoRegister I2cSlv4Do;
            I2cSlv4CtrlRegister I2cSlv4Ctrl;
            I2cSlv4DiRegister I2cSlv4Di;
            I2cMstStatusRegister I2cMstStatus;
            IntPinCfgRegister IntPinCfg;
            IntEnableRegister IntEnable;
            IntStatusRegister IntStatus;
            Accel_xRegister Accel_x;
            Accel_yRegister Accel_y;
            Accel_zRegister Accel_z;
            TempRegister Temp;
            Gyro_xRegister Gyro_x;
            Gyro_yRegister Gyro_y;
            Gyro_zRegister Gyro_z;
            ExtSensDataRegister ExtSensData;
            I2cSlv0doRegister I2cSlv0do;
            I2cSlv1doRegister I2cSlv1do;
            I2cSlv2doRegister I2cSlv2do;
            I2cSlv3doRegister I2cSlv3do;
            I2cMstDelayCtrlRegister I2cMstDelayCtrl;
            SignalPathResetRegister SignalPathReset;
            AccelIntCtrlRegister AccelIntCtrl;
            UserCtrlRegister UserCtrl;
            PowerMgmt1Register PowerMgmt1;
            PowerMgmt2Register PowerMgmt2;
            FifoCountRegister FifoCount;
            FifoRWRegister FifoRW;
            WhoAmIRegister WhoAmI;
            XAccelOffsetRegister XAccelOffset;
            YAccelOffsetRegister YAccelOffset;
            ZAccelOffsetRegister ZAccelOffset;

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
            Accel_x.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            Accel_y.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            Accel_z.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            Temp.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            Gyro_x.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            Gyro_y.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            Gyro_z.setValue(getValue(&bytes[i], 2, ));
            i += 2;
        }

    };
 }  }  } }