#pragma once
#include "SMBusDevice.h"
#include "Treehopper.Libraries.h"

using namespace Treehopper::Libraries;

namespace Treehopper { namespace Libraries { namespace IO { namespace Adc { 
    class Nau7802Registers
    {
        private:
            SMBusDevice& _dev;

        public:
            Nau7802Registers(SMBusDevice& device) : _dev(device)
            {

            }

            class PuCtrlRegister
            {
                public:
                int RegisterReset;
                int PowerUpDigital;
                int PowerUpAnalog;
                int PowerUpReady;
                int CycleStart;
                int CycleReady;
                int UseExternalCrystal;
                int UseInternalLdo;

                long getValue() { return ((RegisterReset & 0x1) << 0) | ((PowerUpDigital & 0x1) << 1) | ((PowerUpAnalog & 0x1) << 2) | ((PowerUpReady & 0x1) << 3) | ((CycleStart & 0x1) << 4) | ((CycleReady & 0x1) << 5) | ((UseExternalCrystal & 0x1) << 6) | ((UseInternalLdo & 0x1) << 7); }
                void setValue(long value)
                {
                    RegisterReset = (int)((value >> 0) & 0x1);
                    PowerUpDigital = (int)((value >> 1) & 0x1);
                    PowerUpAnalog = (int)((value >> 2) & 0x1);
                    PowerUpReady = (int)((value >> 3) & 0x1);
                    CycleStart = (int)((value >> 4) & 0x1);
                    CycleReady = (int)((value >> 5) & 0x1);
                    UseExternalCrystal = (int)((value >> 6) & 0x1);
                    UseInternalLdo = (int)((value >> 7) & 0x1);
                }
            };

            class Ctrl1Register
            {
                public:
                int Gain;
                int Vldo;
                int DrdySelect;
                int ConversionReadyPinPolarity;

                long getValue() { return ((Gain & 0x7) << 0) | ((Vldo & 0x7) << 3) | ((DrdySelect & 0x1) << 6) | ((ConversionReadyPinPolarity & 0x1) << 7); }
                void setValue(long value)
                {
                    Gain = (int)((value >> 0) & 0x7);
                    Vldo = (int)((value >> 3) & 0x7);
                    DrdySelect = (int)((value >> 6) & 0x1);
                    ConversionReadyPinPolarity = (int)((value >> 7) & 0x1);
                }
            };

            class Ctrl2Register
            {
                public:
                int CalMod;
                int CalStart;
                int CalError;
                int ConversionRate;
                int ChannelSelect;

                long getValue() { return ((CalMod & 0x3) << 0) | ((CalStart & 0x1) << 2) | ((CalError & 0x1) << 3) | ((ConversionRate & 0x7) << 4) | ((ChannelSelect & 0x1) << 7); }
                void setValue(long value)
                {
                    CalMod = (int)((value >> 0) & 0x3);
                    CalStart = (int)((value >> 2) & 0x1);
                    CalError = (int)((value >> 3) & 0x1);
                    ConversionRate = (int)((value >> 4) & 0x7);
                    ChannelSelect = (int)((value >> 7) & 0x1);
                }
            };

            class I2cCtrlRegister
            {
                public:
                int BgpCp;
                int Ts;
                int BoPga;
                int Si;
                int Wpd;
                int Spe;
                int Frd;
                int Crsd;

                long getValue() { return ((BgpCp & 0x1) << 0) | ((Ts & 0x1) << 1) | ((BoPga & 0x1) << 2) | ((Si & 0x1) << 3) | ((Wpd & 0x1) << 4) | ((Spe & 0x1) << 5) | ((Frd & 0x1) << 6) | ((Crsd & 0x1) << 7); }
                void setValue(long value)
                {
                    BgpCp = (int)((value >> 0) & 0x1);
                    Ts = (int)((value >> 1) & 0x1);
                    BoPga = (int)((value >> 2) & 0x1);
                    Si = (int)((value >> 3) & 0x1);
                    Wpd = (int)((value >> 4) & 0x1);
                    Spe = (int)((value >> 5) & 0x1);
                    Frd = (int)((value >> 6) & 0x1);
                    Crsd = (int)((value >> 7) & 0x1);
                }
            };

            class AdcResultRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFFFFFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)(((value >> 0) & 0xFFFFFF) << (32 - 24)) >> (32 - 24);
                }
            };

            class AdcRegister
            {
                public:
                int RegChp;
                int AdcVcm;
                int RegChpFreq;

                long getValue() { return ((RegChp & 0x3) << 0) | ((AdcVcm & 0x3) << 2) | ((RegChpFreq & 0x3) << 4); }
                void setValue(long value)
                {
                    RegChp = (int)((value >> 0) & 0x3);
                    AdcVcm = (int)((value >> 2) & 0x3);
                    RegChpFreq = (int)((value >> 4) & 0x3);
                }
            };

            class PgaRegister
            {
                public:
                int DisableChopper;
                int PgaInv;
                int PgaBypass;
                int LdoMode;
                int RdOptSel;

                long getValue() { return ((DisableChopper & 0x1) << 0) | ((PgaInv & 0x1) << 3) | ((PgaBypass & 0x1) << 4) | ((LdoMode & 0x1) << 5) | ((RdOptSel & 0x1) << 6); }
                void setValue(long value)
                {
                    DisableChopper = (int)((value >> 0) & 0x1);
                    PgaInv = (int)((value >> 3) & 0x1);
                    PgaBypass = (int)((value >> 4) & 0x1);
                    LdoMode = (int)((value >> 5) & 0x1);
                    RdOptSel = (int)((value >> 6) & 0x1);
                }
            };

            class PowerCtrlRegister
            {
                public:
                int PgaCurr;
                int AdcCurr;
                int MasterBiasCurr;
                int PgaCapEn;

                long getValue() { return ((PgaCurr & 0x3) << 0) | ((AdcCurr & 0x3) << 2) | ((MasterBiasCurr & 0x7) << 4) | ((PgaCapEn & 0x1) << 7); }
                void setValue(long value)
                {
                    PgaCurr = (int)((value >> 0) & 0x3);
                    AdcCurr = (int)((value >> 2) & 0x3);
                    MasterBiasCurr = (int)((value >> 4) & 0x7);
                    PgaCapEn = (int)((value >> 7) & 0x1);
                }
            };

            PuCtrlRegister PuCtrl;
            Ctrl1Register Ctrl1;
            Ctrl2Register Ctrl2;
            I2cCtrlRegister I2cCtrl;
            AdcResultRegister AdcResult;
            AdcRegister Adc;
            PgaRegister Pga;
            PowerCtrlRegister PowerCtrl;

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
            getBytes(PuCtrl.getValue(), 1, , bytes);
            _dev.writeBufferData(0x00, bytes, 1);
            getBytes(Ctrl1.getValue(), 1, , bytes);
            _dev.writeBufferData(0x01, bytes, 1);
            getBytes(Ctrl2.getValue(), 1, , bytes);
            _dev.writeBufferData(0x02, bytes, 1);
            getBytes(I2cCtrl.getValue(), 1, , bytes);
            _dev.writeBufferData(0x11, bytes, 1);
            getBytes(Adc.getValue(), 1, , bytes);
            _dev.writeBufferData(0x15, bytes, 1);
            getBytes(Pga.getValue(), 1, , bytes);
            _dev.writeBufferData(0x1B, bytes, 1);
            getBytes(PowerCtrl.getValue(), 1, , bytes);
            _dev.writeBufferData(0x1C, bytes, 1);
        }

        void update()
        {
            uint8_t bytes[3];
            int i = 0;
            _dev.readBufferData(18, bytes, 3);
            AdcResult.setValue(getValue(&bytes[i], 3, ));
            i += 3;
        }

    };
 }  }  } }