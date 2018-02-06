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
                int registerReset;
                int powerUpDigital;
                int powerUpAnalog;
                int powerUpReady;
                int cycleStart;
                int cycleReady;
                int useExternalCrystal;
                int useInternalLdo;

                long getValue() { return ((RegisterReset & 0x1) << 0) | ((PowerUpDigital & 0x1) << 1) | ((PowerUpAnalog & 0x1) << 2) | ((PowerUpReady & 0x1) << 3) | ((CycleStart & 0x1) << 4) | ((CycleReady & 0x1) << 5) | ((UseExternalCrystal & 0x1) << 6) | ((UseInternalLdo & 0x1) << 7); }
                void setValue(long value)
                {
                    registerReset = (int)((value >> 0) & 0x1);
                    powerUpDigital = (int)((value >> 1) & 0x1);
                    powerUpAnalog = (int)((value >> 2) & 0x1);
                    powerUpReady = (int)((value >> 3) & 0x1);
                    cycleStart = (int)((value >> 4) & 0x1);
                    cycleReady = (int)((value >> 5) & 0x1);
                    useExternalCrystal = (int)((value >> 6) & 0x1);
                    useInternalLdo = (int)((value >> 7) & 0x1);
                }
            };

            class Ctrl1Register
            {
                public:
                int gain;
                int vldo;
                int drdySelect;
                int conversionReadyPinPolarity;

                long getValue() { return ((Gain & 0x7) << 0) | ((Vldo & 0x7) << 3) | ((DrdySelect & 0x1) << 6) | ((ConversionReadyPinPolarity & 0x1) << 7); }
                void setValue(long value)
                {
                    gain = (int)((value >> 0) & 0x7);
                    vldo = (int)((value >> 3) & 0x7);
                    drdySelect = (int)((value >> 6) & 0x1);
                    conversionReadyPinPolarity = (int)((value >> 7) & 0x1);
                }
            };

            class Ctrl2Register
            {
                public:
                int calMod;
                int calStart;
                int calError;
                int conversionRate;
                int channelSelect;

                long getValue() { return ((CalMod & 0x3) << 0) | ((CalStart & 0x1) << 2) | ((CalError & 0x1) << 3) | ((ConversionRate & 0x7) << 4) | ((ChannelSelect & 0x1) << 7); }
                void setValue(long value)
                {
                    calMod = (int)((value >> 0) & 0x3);
                    calStart = (int)((value >> 2) & 0x1);
                    calError = (int)((value >> 3) & 0x1);
                    conversionRate = (int)((value >> 4) & 0x7);
                    channelSelect = (int)((value >> 7) & 0x1);
                }
            };

            class I2cCtrlRegister
            {
                public:
                int bgpCp;
                int ts;
                int boPga;
                int si;
                int wpd;
                int spe;
                int frd;
                int crsd;

                long getValue() { return ((BgpCp & 0x1) << 0) | ((Ts & 0x1) << 1) | ((BoPga & 0x1) << 2) | ((Si & 0x1) << 3) | ((Wpd & 0x1) << 4) | ((Spe & 0x1) << 5) | ((Frd & 0x1) << 6) | ((Crsd & 0x1) << 7); }
                void setValue(long value)
                {
                    bgpCp = (int)((value >> 0) & 0x1);
                    ts = (int)((value >> 1) & 0x1);
                    boPga = (int)((value >> 2) & 0x1);
                    si = (int)((value >> 3) & 0x1);
                    wpd = (int)((value >> 4) & 0x1);
                    spe = (int)((value >> 5) & 0x1);
                    frd = (int)((value >> 6) & 0x1);
                    crsd = (int)((value >> 7) & 0x1);
                }
            };

            class AdcResultRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)(((value >> 0) & 0xFFFFFF) << (32 - 24)) >> (32 - 24);
                }
            };

            class AdcRegister
            {
                public:
                int regChp;
                int adcVcm;
                int regChpFreq;

                long getValue() { return ((RegChp & 0x3) << 0) | ((AdcVcm & 0x3) << 2) | ((RegChpFreq & 0x3) << 4); }
                void setValue(long value)
                {
                    regChp = (int)((value >> 0) & 0x3);
                    adcVcm = (int)((value >> 2) & 0x3);
                    regChpFreq = (int)((value >> 4) & 0x3);
                }
            };

            class PgaRegister
            {
                public:
                int disableChopper;
                int pgaInv;
                int pgaBypass;
                int ldoMode;
                int rdOptSel;

                long getValue() { return ((DisableChopper & 0x1) << 0) | ((PgaInv & 0x1) << 3) | ((PgaBypass & 0x1) << 4) | ((LdoMode & 0x1) << 5) | ((RdOptSel & 0x1) << 6); }
                void setValue(long value)
                {
                    disableChopper = (int)((value >> 0) & 0x1);
                    pgaInv = (int)((value >> 3) & 0x1);
                    pgaBypass = (int)((value >> 4) & 0x1);
                    ldoMode = (int)((value >> 5) & 0x1);
                    rdOptSel = (int)((value >> 6) & 0x1);
                }
            };

            class PowerCtrlRegister
            {
                public:
                int pgaCurr;
                int adcCurr;
                int masterBiasCurr;
                int pgaCapEn;

                long getValue() { return ((PgaCurr & 0x3) << 0) | ((AdcCurr & 0x3) << 2) | ((MasterBiasCurr & 0x7) << 4) | ((PgaCapEn & 0x1) << 7); }
                void setValue(long value)
                {
                    pgaCurr = (int)((value >> 0) & 0x3);
                    adcCurr = (int)((value >> 2) & 0x3);
                    masterBiasCurr = (int)((value >> 4) & 0x7);
                    pgaCapEn = (int)((value >> 7) & 0x1);
                }
            };

            PuCtrlRegister puCtrl;
            Ctrl1Register ctrl1;
            Ctrl2Register ctrl2;
            I2cCtrlRegister i2cCtrl;
            AdcResultRegister adcResult;
            AdcRegister adc;
            PgaRegister pga;
            PowerCtrlRegister powerCtrl;

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
            getBytes(puCtrl.getValue(), 1, , bytes);
            _dev.writeBufferData(0x00, bytes, 1);
            getBytes(ctrl1.getValue(), 1, , bytes);
            _dev.writeBufferData(0x01, bytes, 1);
            getBytes(ctrl2.getValue(), 1, , bytes);
            _dev.writeBufferData(0x02, bytes, 1);
            getBytes(i2cCtrl.getValue(), 1, , bytes);
            _dev.writeBufferData(0x11, bytes, 1);
            getBytes(adc.getValue(), 1, , bytes);
            _dev.writeBufferData(0x15, bytes, 1);
            getBytes(pga.getValue(), 1, , bytes);
            _dev.writeBufferData(0x1B, bytes, 1);
            getBytes(powerCtrl.getValue(), 1, , bytes);
            _dev.writeBufferData(0x1C, bytes, 1);
        }

        void update()
        {
            uint8_t bytes[3];
            int i = 0;
            _dev.readBufferData(18, bytes, 3);
            adcResult.setValue(getValue(&bytes[i], 3, ));
            i += 3;
        }

    };
 }  }  } }