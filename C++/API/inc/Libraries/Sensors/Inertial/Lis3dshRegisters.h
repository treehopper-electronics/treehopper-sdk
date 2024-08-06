/// This file was auto-generated by RegisterGenerator. Any changes to it will be overwritten!
#pragma once
#include "SMBusDevice.h"
#include "Libraries/Treehopper.Libraries.h"
#include "Libraries/RegisterManager.h"
#include "Libraries/Register.h"

using namespace Treehopper::Libraries;

namespace Treehopper { namespace Libraries { namespace Sensors { namespace Inertial { 

    enum class OutputDataRates
    {
        PowerDown = 0,
        Hz_3P125 = 1,
        Hz_6P25 = 2,
        Hz_12P5 = 3,
        Hz_25 = 4,
        Hz_50 = 5,
        Hz_100 = 6,
        Hz_400 = 7,
        Hz_800 = 8,
        Hz_1600 = 9
	};

    enum class Fscales
    {
        g_2 = 0,
        g_4 = 1,
        g_6 = 2,
        g_8 = 3,
        g_16 = 4
	};

    enum class Bandwidths
    {
        Hz_800 = 0,
        Hz_200 = 1,
        Hz_400 = 2,
        Hz_50 = 3
	};

    enum class FifoModes
    {
        bypass = 0,
        fifoMode = 1,
        streamMode = 2,
        streamUntilTriggerThenFifo = 3,
        bypassUntilTriggerThenStream = 4,
        bypassUntilTriggerThenFIFO = 7
	};


    class Lis3dshRegisters : public RegisterManager
    {
    public:
        class OutTRegister : public Register
        {
        public:
			OutTRegister(RegisterManager& regManager) : Register(regManager,0x0c, 1, false) { }
            int value;

            long getValue() { return ((value & 0xFF) << 0); }
            void setValue(long val)
            {
                value = (int)((val >> 0) & 0xFF);
            }
        };

        class Info1Register : public Register
        {
        public:
			Info1Register(RegisterManager& regManager) : Register(regManager,0x0d, 1, false) { }
            int value;

            long getValue() { return ((value & 0xFF) << 0); }
            void setValue(long val)
            {
                value = (int)((val >> 0) & 0xFF);
            }
        };

        class Info2Register : public Register
        {
        public:
			Info2Register(RegisterManager& regManager) : Register(regManager,0x0e, 1, false) { }
            int value;

            long getValue() { return ((value & 0xFF) << 0); }
            void setValue(long val)
            {
                value = (int)((val >> 0) & 0xFF);
            }
        };

        class WhoAmIRegister : public Register
        {
        public:
			WhoAmIRegister(RegisterManager& regManager) : Register(regManager,0x0f, 1, false) { }
            int value;

            long getValue() { return ((value & 0xFF) << 0); }
            void setValue(long val)
            {
                value = (int)((val >> 0) & 0xFF);
            }
        };

        class OffXRegister : public Register
        {
        public:
			OffXRegister(RegisterManager& regManager) : Register(regManager,0x10, 1, false) { }
            int value;

            long getValue() { return ((value & 0xFF) << 0); }
            void setValue(long val)
            {
                value = (int)((val >> 0) & 0xFF);
            }
        };

        class OffYRegister : public Register
        {
        public:
			OffYRegister(RegisterManager& regManager) : Register(regManager,0x11, 1, false) { }
            int value;

            long getValue() { return ((value & 0xFF) << 0); }
            void setValue(long val)
            {
                value = (int)((val >> 0) & 0xFF);
            }
        };

        class OffZRegister : public Register
        {
        public:
			OffZRegister(RegisterManager& regManager) : Register(regManager,0x12, 1, false) { }
            int value;

            long getValue() { return ((value & 0xFF) << 0); }
            void setValue(long val)
            {
                value = (int)((val >> 0) & 0xFF);
            }
        };

        class CsXRegister : public Register
        {
        public:
			CsXRegister(RegisterManager& regManager) : Register(regManager,0x13, 1, false) { }
            int value;

            long getValue() { return ((value & 0xFF) << 0); }
            void setValue(long val)
            {
                value = (int)((val >> 0) & 0xFF);
            }
        };

        class CsYRegister : public Register
        {
        public:
			CsYRegister(RegisterManager& regManager) : Register(regManager,0x14, 1, false) { }
            int value;

            long getValue() { return ((value & 0xFF) << 0); }
            void setValue(long val)
            {
                value = (int)((val >> 0) & 0xFF);
            }
        };

        class CsZRegister : public Register
        {
        public:
			CsZRegister(RegisterManager& regManager) : Register(regManager,0x15, 1, false) { }
            int value;

            long getValue() { return ((value & 0xFF) << 0); }
            void setValue(long val)
            {
                value = (int)((val >> 0) & 0xFF);
            }
        };

        class LcRegister : public Register
        {
        public:
			LcRegister(RegisterManager& regManager) : Register(regManager,0x16, 2, false) { }
            int value;

            long getValue() { return ((value & 0xFFFF) << 0); }
            void setValue(long val)
            {
                value = (int)((val >> 0) & 0xFFFF);
            }
        };

        class StatRegister : public Register
        {
        public:
			StatRegister(RegisterManager& regManager) : Register(regManager,0x18, 1, false) { }
            int drdy;
            int dor;
            int intSm2;
            int intSm1;
            int sync2;
            int sync1;
            int syncw;
            int longInterrupt;

            long getValue() { return ((drdy & 0x1) << 0) | ((dor & 0x1) << 1) | ((intSm2 & 0x1) << 2) | ((intSm1 & 0x1) << 3) | ((sync2 & 0x1) << 4) | ((sync1 & 0x1) << 5) | ((syncw & 0x1) << 6) | ((longInterrupt & 0x1) << 7); }
            void setValue(long val)
            {
                drdy = (int)((val >> 0) & 0x1);
                dor = (int)((val >> 1) & 0x1);
                intSm2 = (int)((val >> 2) & 0x1);
                intSm1 = (int)((val >> 3) & 0x1);
                sync2 = (int)((val >> 4) & 0x1);
                sync1 = (int)((val >> 5) & 0x1);
                syncw = (int)((val >> 6) & 0x1);
                longInterrupt = (int)((val >> 7) & 0x1);
            }
        };

        class Peak1Register : public Register
        {
        public:
			Peak1Register(RegisterManager& regManager) : Register(regManager,0x19, 1, false) { }
            int value;

            long getValue() { return ((value & 0xFF) << 0); }
            void setValue(long val)
            {
                value = (int)((val >> 0) & 0xFF);
            }
        };

        class Peak2Register : public Register
        {
        public:
			Peak2Register(RegisterManager& regManager) : Register(regManager,0x1A, 1, false) { }
            int value;

            long getValue() { return ((value & 0xFF) << 0); }
            void setValue(long val)
            {
                value = (int)((val >> 0) & 0xFF);
            }
        };

        class Vfc1Register : public Register
        {
        public:
			Vfc1Register(RegisterManager& regManager) : Register(regManager,0x1b, 1, false) { }
            int value;

            long getValue() { return ((value & 0xFF) << 0); }
            void setValue(long val)
            {
                value = (int)((val >> 0) & 0xFF);
            }
        };

        class Vfc2Register : public Register
        {
        public:
			Vfc2Register(RegisterManager& regManager) : Register(regManager,0x1c, 1, false) { }
            int value;

            long getValue() { return ((value & 0xFF) << 0); }
            void setValue(long val)
            {
                value = (int)((val >> 0) & 0xFF);
            }
        };

        class Vfc3Register : public Register
        {
        public:
			Vfc3Register(RegisterManager& regManager) : Register(regManager,0x1d, 1, false) { }
            int value;

            long getValue() { return ((value & 0xFF) << 0); }
            void setValue(long val)
            {
                value = (int)((val >> 0) & 0xFF);
            }
        };

        class Vfc4Register : public Register
        {
        public:
			Vfc4Register(RegisterManager& regManager) : Register(regManager,0x1e, 1, false) { }
            int value;

            long getValue() { return ((value & 0xFF) << 0); }
            void setValue(long val)
            {
                value = (int)((val >> 0) & 0xFF);
            }
        };

        class Thrs3Register : public Register
        {
        public:
			Thrs3Register(RegisterManager& regManager) : Register(regManager,0x1f, 1, false) { }
            int value;

            long getValue() { return ((value & 0xFF) << 0); }
            void setValue(long val)
            {
                value = (int)((val >> 0) & 0xFF);
            }
        };

        class CtrlReg4Register : public Register
        {
        public:
			CtrlReg4Register(RegisterManager& regManager) : Register(regManager,0x20, 1, false) { }
            int xen;
            int yen;
            int zen;
            int bdu;
            int outputDataRate;
            OutputDataRates getOutputDataRate() { return (OutputDataRates)outputDataRate; }
            void setOutputDataRate(OutputDataRates enumVal) { outputDataRate = (int)enumVal; }

            long getValue() { return ((xen & 0x1) << 0) | ((yen & 0x1) << 1) | ((zen & 0x1) << 2) | ((bdu & 0x1) << 3) | ((outputDataRate & 0xF) << 4); }
            void setValue(long val)
            {
                xen = (int)((val >> 0) & 0x1);
                yen = (int)((val >> 1) & 0x1);
                zen = (int)((val >> 2) & 0x1);
                bdu = (int)((val >> 3) & 0x1);
                outputDataRate = (int)((val >> 4) & 0xF);
            }
        };

        class CtrlReg1Register : public Register
        {
        public:
			CtrlReg1Register(RegisterManager& regManager) : Register(regManager,0x21, 1, false) { }
            int sm1En;
            int sm1Pin;
            int Hyst1;

            long getValue() { return ((sm1En & 0x1) << 0) | ((sm1Pin & 0x1) << 3) | ((Hyst1 & 0x7) << 5); }
            void setValue(long val)
            {
                sm1En = (int)((val >> 0) & 0x1);
                sm1Pin = (int)((val >> 3) & 0x1);
                Hyst1 = (int)((val >> 5) & 0x7);
            }
        };

        class CtrlReg2Register : public Register
        {
        public:
			CtrlReg2Register(RegisterManager& regManager) : Register(regManager,0x22, 1, false) { }
            int sm2En;
            int sm2Pin;
            int Hyst2;

            long getValue() { return ((sm2En & 0x1) << 0) | ((sm2Pin & 0x1) << 3) | ((Hyst2 & 0x7) << 5); }
            void setValue(long val)
            {
                sm2En = (int)((val >> 0) & 0x1);
                sm2Pin = (int)((val >> 3) & 0x1);
                Hyst2 = (int)((val >> 5) & 0x7);
            }
        };

        class CtrlReg3Register : public Register
        {
        public:
			CtrlReg3Register(RegisterManager& regManager) : Register(regManager,0x23, 1, false) { }
            int strt;
            int vfilt;
            int int1En;
            int int2En;
            int iel;
            int iea;
            int drEn;

            long getValue() { return ((strt & 0x1) << 0) | ((vfilt & 0x1) << 2) | ((int1En & 0x1) << 3) | ((int2En & 0x1) << 4) | ((iel & 0x1) << 5) | ((iea & 0x1) << 6) | ((drEn & 0x1) << 7); }
            void setValue(long val)
            {
                strt = (int)((val >> 0) & 0x1);
                vfilt = (int)((val >> 2) & 0x1);
                int1En = (int)((val >> 3) & 0x1);
                int2En = (int)((val >> 4) & 0x1);
                iel = (int)((val >> 5) & 0x1);
                iea = (int)((val >> 6) & 0x1);
                drEn = (int)((val >> 7) & 0x1);
            }
        };

        class CtrlReg5Register : public Register
        {
        public:
			CtrlReg5Register(RegisterManager& regManager) : Register(regManager,0x24, 2, false) { }
            int sim;
            int st1;
            int st2;
            int fscale;
            int bandwidth;
            Fscales getFscale() { return (Fscales)fscale; }
            void setFscale(Fscales enumVal) { fscale = (int)enumVal; }
            Bandwidths getBandwidth() { return (Bandwidths)bandwidth; }
            void setBandwidth(Bandwidths enumVal) { bandwidth = (int)enumVal; }

            long getValue() { return ((sim & 0x1) << 0) | ((st1 & 0x1) << 1) | ((st2 & 0x1) << 2) | ((fscale & 0x7) << 3) | ((bandwidth & 0x7) << 6); }
            void setValue(long val)
            {
                sim = (int)((val >> 0) & 0x1);
                st1 = (int)((val >> 1) & 0x1);
                st2 = (int)((val >> 2) & 0x1);
                fscale = (int)((val >> 3) & 0x7);
                bandwidth = (int)((val >> 6) & 0x7);
            }
        };

        class CtrlReg6Register : public Register
        {
        public:
			CtrlReg6Register(RegisterManager& regManager) : Register(regManager,0x25, 1, false) { }
            int p2boot;
            int p1overrun;
            int p1wtm;
            int p1empty;
            int addInc;
            int wtmEn;
            int fifoEn;
            int boot;

            long getValue() { return ((p2boot & 0x1) << 0) | ((p1overrun & 0x1) << 1) | ((p1wtm & 0x1) << 2) | ((p1empty & 0x1) << 3) | ((addInc & 0x1) << 4) | ((wtmEn & 0x1) << 5) | ((fifoEn & 0x1) << 6) | ((boot & 0x1) << 7); }
            void setValue(long val)
            {
                p2boot = (int)((val >> 0) & 0x1);
                p1overrun = (int)((val >> 1) & 0x1);
                p1wtm = (int)((val >> 2) & 0x1);
                p1empty = (int)((val >> 3) & 0x1);
                addInc = (int)((val >> 4) & 0x1);
                wtmEn = (int)((val >> 5) & 0x1);
                fifoEn = (int)((val >> 6) & 0x1);
                boot = (int)((val >> 7) & 0x1);
            }
        };

        class StatusRegister : public Register
        {
        public:
			StatusRegister(RegisterManager& regManager) : Register(regManager,0x27, 1, false) { }
            int xda;
            int yda;
            int zda;
            int zyxda;
            int xor;
            int yor;
            int zor;
            int zyxor;

            long getValue() { return ((xda & 0x1) << 0) | ((yda & 0x1) << 1) | ((zda & 0x1) << 2) | ((zyxda & 0x1) << 3) | ((xor & 0x1) << 4) | ((yor & 0x1) << 5) | ((zor & 0x1) << 6) | ((zyxor & 0x1) << 7); }
            void setValue(long val)
            {
                xda = (int)((val >> 0) & 0x1);
                yda = (int)((val >> 1) & 0x1);
                zda = (int)((val >> 2) & 0x1);
                zyxda = (int)((val >> 3) & 0x1);
                xor = (int)((val >> 4) & 0x1);
                yor = (int)((val >> 5) & 0x1);
                zor = (int)((val >> 6) & 0x1);
                zyxor = (int)((val >> 7) & 0x1);
            }
        };

        class OutXRegister : public Register
        {
        public:
			OutXRegister(RegisterManager& regManager) : Register(regManager,0x28, 2, false) { }
            int value;

            long getValue() { return ((value & 0xFFFF) << 0); }
            void setValue(long val)
            {
                value = (int)((val >> 0) & 0xFFFF);
            }
        };

        class OutYRegister : public Register
        {
        public:
			OutYRegister(RegisterManager& regManager) : Register(regManager,0x2a, 2, false) { }
            int value;

            long getValue() { return ((value & 0xFFFF) << 0); }
            void setValue(long val)
            {
                value = (int)((val >> 0) & 0xFFFF);
            }
        };

        class OutZRegister : public Register
        {
        public:
			OutZRegister(RegisterManager& regManager) : Register(regManager,0x2c, 2, false) { }
            int value;

            long getValue() { return ((value & 0xFFFF) << 0); }
            void setValue(long val)
            {
                value = (int)((val >> 0) & 0xFFFF);
            }
        };

        class FifoCtrlRegister : public Register
        {
        public:
			FifoCtrlRegister(RegisterManager& regManager) : Register(regManager,0x2e, 1, false) { }
            int wtmp;
            int fifoMode;
            FifoModes getFifoMode() { return (FifoModes)fifoMode; }
            void setFifoMode(FifoModes enumVal) { fifoMode = (int)enumVal; }

            long getValue() { return ((wtmp & 0x1F) << 0) | ((fifoMode & 0x7) << 5); }
            void setValue(long val)
            {
                wtmp = (int)((val >> 0) & 0x1F);
                fifoMode = (int)((val >> 5) & 0x7);
            }
        };

        class FifoSrcRegister : public Register
        {
        public:
			FifoSrcRegister(RegisterManager& regManager) : Register(regManager,0x2f, 1, false) { }
            int fss;
            int empty;
            int ovrnFifo;
            int wtm;

            long getValue() { return ((fss & 0x1F) << 0) | ((empty & 0x1) << 5) | ((ovrnFifo & 0x1) << 6) | ((wtm & 0x1) << 7); }
            void setValue(long val)
            {
                fss = (int)((val >> 0) & 0x1F);
                empty = (int)((val >> 5) & 0x1);
                ovrnFifo = (int)((val >> 6) & 0x1);
                wtm = (int)((val >> 7) & 0x1);
            }
        };

            OutTRegister outT;
            Info1Register info1;
            Info2Register info2;
            WhoAmIRegister whoAmI;
            OffXRegister offX;
            OffYRegister offY;
            OffZRegister offZ;
            CsXRegister csX;
            CsYRegister csY;
            CsZRegister csZ;
            LcRegister lc;
            StatRegister stat;
            Peak1Register peak1;
            Peak2Register peak2;
            Vfc1Register vfc1;
            Vfc2Register vfc2;
            Vfc3Register vfc3;
            Vfc4Register vfc4;
            Thrs3Register thrs3;
            CtrlReg4Register ctrlReg4;
            CtrlReg1Register ctrlReg1;
            CtrlReg2Register ctrlReg2;
            CtrlReg3Register ctrlReg3;
            CtrlReg5Register ctrlReg5;
            CtrlReg6Register ctrlReg6;
            StatusRegister status;
            OutXRegister outX;
            OutYRegister outY;
            OutZRegister outZ;
            FifoCtrlRegister fifoCtrl;
            FifoSrcRegister fifoSrc;

		Lis3dshRegisters(SMBusDevice& device) : RegisterManager(device, true), outT(*this), info1(*this), info2(*this), whoAmI(*this), offX(*this), offY(*this), offZ(*this), csX(*this), csY(*this), csZ(*this), lc(*this), stat(*this), peak1(*this), peak2(*this), vfc1(*this), vfc2(*this), vfc3(*this), vfc4(*this), thrs3(*this), ctrlReg4(*this), ctrlReg1(*this), ctrlReg2(*this), ctrlReg3(*this), ctrlReg5(*this), ctrlReg6(*this), status(*this), outX(*this), outY(*this), outZ(*this), fifoCtrl(*this), fifoSrc(*this)
		{ 
			registers.push_back(&outT);
			registers.push_back(&info1);
			registers.push_back(&info2);
			registers.push_back(&whoAmI);
			registers.push_back(&offX);
			registers.push_back(&offY);
			registers.push_back(&offZ);
			registers.push_back(&csX);
			registers.push_back(&csY);
			registers.push_back(&csZ);
			registers.push_back(&lc);
			registers.push_back(&stat);
			registers.push_back(&peak1);
			registers.push_back(&peak2);
			registers.push_back(&vfc1);
			registers.push_back(&vfc2);
			registers.push_back(&vfc3);
			registers.push_back(&vfc4);
			registers.push_back(&thrs3);
			registers.push_back(&ctrlReg4);
			registers.push_back(&ctrlReg1);
			registers.push_back(&ctrlReg2);
			registers.push_back(&ctrlReg3);
			registers.push_back(&ctrlReg5);
			registers.push_back(&ctrlReg6);
			registers.push_back(&status);
			registers.push_back(&outX);
			registers.push_back(&outY);
			registers.push_back(&outZ);
			registers.push_back(&fifoCtrl);
			registers.push_back(&fifoSrc);
		}
    };
 }  }  } }