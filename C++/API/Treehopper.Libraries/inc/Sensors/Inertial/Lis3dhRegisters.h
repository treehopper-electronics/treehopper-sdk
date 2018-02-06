#pragma once
#include "SMBusDevice.h"
#include "Treehopper.Libraries.h"

using namespace Treehopper::Libraries;

namespace Treehopper { namespace Libraries { namespace Sensors { namespace Inertial { 
    class Lis3dhRegisters
    {
        private:
            SMBusDevice& _dev;

        public:
            Lis3dhRegisters(SMBusDevice& device) : _dev(device)
            {

            }

            class StatusRegAuxRegister
            {
                public:
                int oneAxisDataAvailable;
                int twoAxisDataAvailable;
                int threeAxisDataAvailable;
                int dataAvailable;
                int oneAxisDataOverrun;
                int twoAxisDataOverrun;
                int dataOverrun;

                long getValue() { return ((OneAxisDataAvailable & 0x1) << 0) | ((TwoAxisDataAvailable & 0x1) << 1) | ((ThreeAxisDataAvailable & 0x1) << 2) | ((DataAvailable & 0x1) << 3) | ((OneAxisDataOverrun & 0x1) << 4) | ((TwoAxisDataOverrun & 0x1) << 5) | ((DataOverrun & 0x1) << 6); }
                void setValue(long value)
                {
                    oneAxisDataAvailable = (int)((value >> 0) & 0x1);
                    twoAxisDataAvailable = (int)((value >> 1) & 0x1);
                    threeAxisDataAvailable = (int)((value >> 2) & 0x1);
                    dataAvailable = (int)((value >> 3) & 0x1);
                    oneAxisDataOverrun = (int)((value >> 4) & 0x1);
                    twoAxisDataOverrun = (int)((value >> 5) & 0x1);
                    dataOverrun = (int)((value >> 6) & 0x1);
                }
            };

            class OutAdc1Register
            {
                public:
                int value;

                long getValue() { return ((Value & 0x3FF) << 6); }
                void setValue(long value)
                {
                    value = (int)((value >> 6) & 0x3FF);
                }
            };

            class OutAdc2Register
            {
                public:
                int value;

                long getValue() { return ((Value & 0x3FF) << 6); }
                void setValue(long value)
                {
                    value = (int)((value >> 6) & 0x3FF);
                }
            };

            class OutAdc3Register
            {
                public:
                int value;

                long getValue() { return ((Value & 0x3FF) << 6); }
                void setValue(long value)
                {
                    value = (int)((value >> 6) & 0x3FF);
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

            class Ctrl0Register
            {
                public:
                int sdoPuDisc;

                long getValue() { return ((SdoPuDisc & 0xFF) << 0); }
                void setValue(long value)
                {
                    sdoPuDisc = (int)((value >> 0) & 0xFF);
                }
            };

            class TempCfgRegRegister
            {
                public:
                int adcEn;
                int tempEn;

                long getValue() { return ((AdcEn & 0x1) << 7) | ((TempEn & 0x1) << 6); }
                void setValue(long value)
                {
                    adcEn = (int)((value >> 7) & 0x1);
                    tempEn = (int)((value >> 6) & 0x1);
                }
            };

            class Ctrl2Register
            {
                public:
                int highPassAoiInt1Enable;
                int highPassAoiInt2Enable;
                int highPassClickEnable;
                int filterDataPassThru;
                int highPassFilterCutoffFrequency;
                int highPassFilterModeSelection;

                long getValue() { return ((HighPassAoiInt1Enable & 0x1) << 0) | ((HighPassAoiInt2Enable & 0x1) << 1) | ((HighPassClickEnable & 0x1) << 2) | ((FilterDataPassThru & 0x1) << 3) | ((HighPassFilterCutoffFrequency & 0x3) << 4) | ((HighPassFilterModeSelection & 0x3) << 6); }
                void setValue(long value)
                {
                    highPassAoiInt1Enable = (int)((value >> 0) & 0x1);
                    highPassAoiInt2Enable = (int)((value >> 1) & 0x1);
                    highPassClickEnable = (int)((value >> 2) & 0x1);
                    filterDataPassThru = (int)((value >> 3) & 0x1);
                    highPassFilterCutoffFrequency = (int)((value >> 4) & 0x3);
                    highPassFilterModeSelection = (int)((value >> 6) & 0x3);
                }
            };

            class Ctrl3Register
            {
                public:
                int overrun;
                int fifoWatermark;
                int da321;
                int zyxda;
                int ia2;
                int ia1;
                int click;

                long getValue() { return ((Overrun & 0x1) << 1) | ((FifoWatermark & 0x1) << 2) | ((Da321 & 0x1) << 3) | ((Zyxda & 0x1) << 4) | ((Ia2 & 0x1) << 5) | ((Ia1 & 0x1) << 6) | ((Click & 0x1) << 7); }
                void setValue(long value)
                {
                    overrun = (int)((value >> 1) & 0x1);
                    fifoWatermark = (int)((value >> 2) & 0x1);
                    da321 = (int)((value >> 3) & 0x1);
                    zyxda = (int)((value >> 4) & 0x1);
                    ia2 = (int)((value >> 5) & 0x1);
                    ia1 = (int)((value >> 6) & 0x1);
                    click = (int)((value >> 7) & 0x1);
                }
            };

            class Ctrl4Register
            {
                public:
                int spiInterfaceMode;
                int selfTestEnable;
                int highResolutionOutput;
                int fullScaleSelection;
                int bigEndian;
                int blockDataUpdate;

                long getValue() { return ((SpiInterfaceMode & 0x1) << 0) | ((SelfTestEnable & 0x3) << 1) | ((HighResolutionOutput & 0x1) << 3) | ((FullScaleSelection & 0x3) << 4) | ((BigEndian & 0x1) << 6) | ((BlockDataUpdate & 0x1) << 7); }
                void setValue(long value)
                {
                    spiInterfaceMode = (int)((value >> 0) & 0x1);
                    selfTestEnable = (int)((value >> 1) & 0x3);
                    highResolutionOutput = (int)((value >> 3) & 0x1);
                    fullScaleSelection = (int)((value >> 4) & 0x3);
                    bigEndian = (int)((value >> 6) & 0x1);
                    blockDataUpdate = (int)((value >> 7) & 0x1);
                }
            };

            class Ctrl5Register
            {
                public:
                int enable4DInt2;
                int latchInt2;
                int enable4DInt1;
                int latchInt1;
                int fifoEnable;
                int rebootMemoryContent;

                long getValue() { return ((Enable4DInt2 & 0x1) << 0) | ((LatchInt2 & 0x1) << 1) | ((Enable4DInt1 & 0x1) << 2) | ((LatchInt1 & 0x1) << 3) | ((FifoEnable & 0x1) << 4) | ((RebootMemoryContent & 0x1) << 5); }
                void setValue(long value)
                {
                    enable4DInt2 = (int)((value >> 0) & 0x1);
                    latchInt2 = (int)((value >> 1) & 0x1);
                    enable4DInt1 = (int)((value >> 2) & 0x1);
                    latchInt1 = (int)((value >> 3) & 0x1);
                    fifoEnable = (int)((value >> 4) & 0x1);
                    rebootMemoryContent = (int)((value >> 5) & 0x1);
                }
            };

            class Ctrl6Register
            {
                public:
                int intPolarity;
                int act;
                int boot;
                int ia2;
                int ia1;
                int click;

                long getValue() { return ((IntPolarity & 0x1) << 1) | ((Act & 0x1) << 3) | ((Boot & 0x1) << 4) | ((Ia2 & 0x1) << 5) | ((Ia1 & 0x1) << 6) | ((Click & 0x1) << 7); }
                void setValue(long value)
                {
                    intPolarity = (int)((value >> 1) & 0x1);
                    act = (int)((value >> 3) & 0x1);
                    boot = (int)((value >> 4) & 0x1);
                    ia2 = (int)((value >> 5) & 0x1);
                    ia1 = (int)((value >> 6) & 0x1);
                    click = (int)((value >> 7) & 0x1);
                }
            };

            class ReferenceRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class StatusRegister
            {
                public:
                int xda;
                int yda;
                int zda;
                int zyxda;
                int xor;
                int yor;
                int zor;
                int zyxor;

                long getValue() { return ((Xda & 0x1) << 0) | ((Yda & 0x1) << 1) | ((Zda & 0x1) << 2) | ((Zyxda & 0x1) << 3) | ((Xor & 0x1) << 4) | ((Yor & 0x1) << 5) | ((Zor & 0x1) << 6) | ((Zyxor & 0x1) << 7); }
                void setValue(long value)
                {
                    xda = (int)((value >> 0) & 0x1);
                    yda = (int)((value >> 1) & 0x1);
                    zda = (int)((value >> 2) & 0x1);
                    zyxda = (int)((value >> 3) & 0x1);
                    xor = (int)((value >> 4) & 0x1);
                    yor = (int)((value >> 5) & 0x1);
                    zor = (int)((value >> 6) & 0x1);
                    zyxor = (int)((value >> 7) & 0x1);
                }
            };

            class FifoCtrlRegister
            {
                public:
                int fifoThreshold;
                int triggerSelection;
                int fifoMode;

                long getValue() { return ((FifoThreshold & 0x1F) << 0) | ((TriggerSelection & 0x1) << 5) | ((FifoMode & 0x3) << 6); }
                void setValue(long value)
                {
                    fifoThreshold = (int)((value >> 0) & 0x1F);
                    triggerSelection = (int)((value >> 5) & 0x1);
                    fifoMode = (int)((value >> 6) & 0x3);
                }
            };

            class FifoSrcRegister
            {
                public:
                int fss;
                int emtpy;
                int overrunFifo;
                int watermark;

                long getValue() { return ((Fss & 0x1F) << 0) | ((Emtpy & 0x1) << 5) | ((OverrunFifo & 0x1) << 6) | ((Watermark & 0x1) << 7); }
                void setValue(long value)
                {
                    fss = (int)((value >> 0) & 0x1F);
                    emtpy = (int)((value >> 5) & 0x1);
                    overrunFifo = (int)((value >> 6) & 0x1);
                    watermark = (int)((value >> 7) & 0x1);
                }
            };

            class Int1CfgRegister
            {
                public:
                int enableXLowEvent;
                int enableXHighEvent;
                int enableYLowEvent;
                int enableYHighEvent;
                int enableZLowEvent;
                int enableZHighEvent;
                int enable6D;
                int andOrInterruptEvents;

                long getValue() { return ((EnableXLowEvent & 0x1) << 0) | ((EnableXHighEvent & 0x1) << 1) | ((EnableYLowEvent & 0x1) << 2) | ((EnableYHighEvent & 0x1) << 3) | ((EnableZLowEvent & 0x1) << 4) | ((EnableZHighEvent & 0x1) << 5) | ((Enable6D & 0x1) << 6) | ((AndOrInterruptEvents & 0x1) << 7); }
                void setValue(long value)
                {
                    enableXLowEvent = (int)((value >> 0) & 0x1);
                    enableXHighEvent = (int)((value >> 1) & 0x1);
                    enableYLowEvent = (int)((value >> 2) & 0x1);
                    enableYHighEvent = (int)((value >> 3) & 0x1);
                    enableZLowEvent = (int)((value >> 4) & 0x1);
                    enableZHighEvent = (int)((value >> 5) & 0x1);
                    enable6D = (int)((value >> 6) & 0x1);
                    andOrInterruptEvents = (int)((value >> 7) & 0x1);
                }
            };

            class Int1SrcRegister
            {
                public:
                int xLow;
                int xHigh;
                int yLow;
                int yHigh;
                int zLow;
                int zHigh;
                int interruptActive;

                long getValue() { return ((XLow & 0x1) << 0) | ((XHigh & 0x1) << 1) | ((YLow & 0x1) << 2) | ((YHigh & 0x1) << 3) | ((ZLow & 0x1) << 4) | ((ZHigh & 0x1) << 5) | ((InterruptActive & 0x1) << 6); }
                void setValue(long value)
                {
                    xLow = (int)((value >> 0) & 0x1);
                    xHigh = (int)((value >> 1) & 0x1);
                    yLow = (int)((value >> 2) & 0x1);
                    yHigh = (int)((value >> 3) & 0x1);
                    zLow = (int)((value >> 4) & 0x1);
                    zHigh = (int)((value >> 5) & 0x1);
                    interruptActive = (int)((value >> 6) & 0x1);
                }
            };

            class Int1ThresholdRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0x7F) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0x7F);
                }
            };

            class Int1DurationRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0x7F) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0x7F);
                }
            };

            class Int2CfgRegister
            {
                public:
                int enableXLowEvent;
                int enableXHighEvent;
                int enableYLowEvent;
                int enableYHighEvent;
                int enableZLowEvent;
                int enableZHighEvent;
                int enable6D;
                int andOrInterruptEvents;

                long getValue() { return ((EnableXLowEvent & 0x1) << 0) | ((EnableXHighEvent & 0x1) << 1) | ((EnableYLowEvent & 0x1) << 2) | ((EnableYHighEvent & 0x1) << 3) | ((EnableZLowEvent & 0x1) << 4) | ((EnableZHighEvent & 0x1) << 5) | ((Enable6D & 0x1) << 6) | ((AndOrInterruptEvents & 0x1) << 7); }
                void setValue(long value)
                {
                    enableXLowEvent = (int)((value >> 0) & 0x1);
                    enableXHighEvent = (int)((value >> 1) & 0x1);
                    enableYLowEvent = (int)((value >> 2) & 0x1);
                    enableYHighEvent = (int)((value >> 3) & 0x1);
                    enableZLowEvent = (int)((value >> 4) & 0x1);
                    enableZHighEvent = (int)((value >> 5) & 0x1);
                    enable6D = (int)((value >> 6) & 0x1);
                    andOrInterruptEvents = (int)((value >> 7) & 0x1);
                }
            };

            class Int2SrcRegister
            {
                public:
                int xLow;
                int xHigh;
                int yLow;
                int yHigh;
                int zLow;
                int zHigh;
                int interruptActive;

                long getValue() { return ((XLow & 0x1) << 0) | ((XHigh & 0x1) << 1) | ((YLow & 0x1) << 2) | ((YHigh & 0x1) << 3) | ((ZLow & 0x1) << 4) | ((ZHigh & 0x1) << 5) | ((InterruptActive & 0x1) << 6); }
                void setValue(long value)
                {
                    xLow = (int)((value >> 0) & 0x1);
                    xHigh = (int)((value >> 1) & 0x1);
                    yLow = (int)((value >> 2) & 0x1);
                    yHigh = (int)((value >> 3) & 0x1);
                    zLow = (int)((value >> 4) & 0x1);
                    zHigh = (int)((value >> 5) & 0x1);
                    interruptActive = (int)((value >> 6) & 0x1);
                }
            };

            class Int2ThresholdRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0x7F) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0x7F);
                }
            };

            class Int2DurationRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0x7F) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0x7F);
                }
            };

            class ClickSourceRegister
            {
                public:
                int x;
                int y;
                int z;
                int sign;
                int singleClickEnable;
                int doubleClickEnable;
                int interruptActive;

                long getValue() { return ((X & 0x1) << 0) | ((Y & 0x1) << 1) | ((Z & 0x1) << 2) | ((Sign & 0x1) << 3) | ((SingleClickEnable & 0x1) << 4) | ((DoubleClickEnable & 0x1) << 5) | ((InterruptActive & 0x1) << 6); }
                void setValue(long value)
                {
                    x = (int)((value >> 0) & 0x1);
                    y = (int)((value >> 1) & 0x1);
                    z = (int)((value >> 2) & 0x1);
                    sign = (int)((value >> 3) & 0x1);
                    singleClickEnable = (int)((value >> 4) & 0x1);
                    doubleClickEnable = (int)((value >> 5) & 0x1);
                    interruptActive = (int)((value >> 6) & 0x1);
                }
            };

            class ClickThresholdRegister
            {
                public:
                int threshold;
                int lirClick;

                long getValue() { return ((Threshold & 0x7F) << 0) | ((LirClick & 0x1) << 7); }
                void setValue(long value)
                {
                    threshold = (int)((value >> 0) & 0x7F);
                    lirClick = (int)((value >> 7) & 0x1);
                }
            };

            class TimeLimitRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0x7F) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0x7F);
                }
            };

            class TimeLatencyRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0x7FFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0x7FFF);
                }
            };

            class TimeWindowRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class ActivationThresholdRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0x7F) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0x7F);
                }
            };

            class ActivationDurationRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class Ctrl1Register
            {
                public:
                int xAxisEnable;
                int yAxisEnable;
                int zAxisEnable;
                int lowPowerEnable;
                int outputDataRate;

                long getValue() { return ((XAxisEnable & 0x1) << 0) | ((YAxisEnable & 0x1) << 1) | ((ZAxisEnable & 0x1) << 2) | ((LowPowerEnable & 0x1) << 3) | ((OutputDataRate & 0xF) << 4); }
                void setValue(long value)
                {
                    xAxisEnable = (int)((value >> 0) & 0x1);
                    yAxisEnable = (int)((value >> 1) & 0x1);
                    zAxisEnable = (int)((value >> 2) & 0x1);
                    lowPowerEnable = (int)((value >> 3) & 0x1);
                    outputDataRate = (int)((value >> 4) & 0xF);
                }
            };

            class OutXRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0x3FF) << 6); }
                void setValue(long value)
                {
                    value = (int)(((value >> 6) & 0x3FF) << (32 - 10)) >> (32 - 10);
                }
            };

            class OutYRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0x3FF) << 6); }
                void setValue(long value)
                {
                    value = (int)(((value >> 6) & 0x3FF) << (32 - 10)) >> (32 - 10);
                }
            };

            class OutZRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0x3FF) << 6); }
                void setValue(long value)
                {
                    value = (int)(((value >> 6) & 0x3FF) << (32 - 10)) >> (32 - 10);
                }
            };

            StatusRegAuxRegister statusRegAux;
            OutAdc1Register outAdc1;
            OutAdc2Register outAdc2;
            OutAdc3Register outAdc3;
            WhoAmIRegister whoAmI;
            Ctrl0Register ctrl0;
            TempCfgRegRegister tempCfgReg;
            Ctrl2Register ctrl2;
            Ctrl3Register ctrl3;
            Ctrl4Register ctrl4;
            Ctrl5Register ctrl5;
            Ctrl6Register ctrl6;
            ReferenceRegister reference;
            StatusRegister status;
            FifoCtrlRegister fifoCtrl;
            FifoSrcRegister fifoSrc;
            Int1CfgRegister int1Cfg;
            Int1SrcRegister int1Src;
            Int1ThresholdRegister int1Threshold;
            Int1DurationRegister int1Duration;
            Int2CfgRegister int2Cfg;
            Int2SrcRegister int2Src;
            Int2ThresholdRegister int2Threshold;
            Int2DurationRegister int2Duration;
            ClickSourceRegister clickSource;
            ClickThresholdRegister clickThreshold;
            TimeLimitRegister timeLimit;
            TimeLatencyRegister timeLatency;
            TimeWindowRegister timeWindow;
            ActivationThresholdRegister activationThreshold;
            ActivationDurationRegister activationDuration;
            Ctrl1Register ctrl1;
            OutXRegister outX;
            OutYRegister outY;
            OutZRegister outZ;

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
            getBytes(ctrl0.getValue(), 1, , bytes);
            _dev.writeBufferData(0x1E, bytes, 1);
            getBytes(tempCfgReg.getValue(), 1, , bytes);
            _dev.writeBufferData(0x1F, bytes, 1);
            getBytes(ctrl2.getValue(), 1, , bytes);
            _dev.writeBufferData(0x21, bytes, 1);
            getBytes(ctrl3.getValue(), 1, , bytes);
            _dev.writeBufferData(0x22, bytes, 1);
            getBytes(ctrl4.getValue(), 1, , bytes);
            _dev.writeBufferData(0x23, bytes, 1);
            getBytes(ctrl5.getValue(), 1, , bytes);
            _dev.writeBufferData(0x24, bytes, 1);
            getBytes(ctrl6.getValue(), 1, , bytes);
            _dev.writeBufferData(0x25, bytes, 1);
            getBytes(reference.getValue(), 1, , bytes);
            _dev.writeBufferData(0x26, bytes, 1);
            getBytes(status.getValue(), 1, , bytes);
            _dev.writeBufferData(0x27, bytes, 1);
            getBytes(fifoCtrl.getValue(), 1, , bytes);
            _dev.writeBufferData(0x2E, bytes, 1);
            getBytes(int1Cfg.getValue(), 1, , bytes);
            _dev.writeBufferData(0x30, bytes, 1);
            getBytes(int1Threshold.getValue(), 1, , bytes);
            _dev.writeBufferData(0x32, bytes, 1);
            getBytes(int1Duration.getValue(), 1, , bytes);
            _dev.writeBufferData(0x33, bytes, 1);
            getBytes(int2Cfg.getValue(), 1, , bytes);
            _dev.writeBufferData(0x34, bytes, 1);
            getBytes(int2Threshold.getValue(), 1, , bytes);
            _dev.writeBufferData(0x36, bytes, 1);
            getBytes(int2Duration.getValue(), 1, , bytes);
            _dev.writeBufferData(0x37, bytes, 1);
            getBytes(clickSource.getValue(), 1, , bytes);
            _dev.writeBufferData(0x39, bytes, 1);
            getBytes(clickThreshold.getValue(), 1, , bytes);
            _dev.writeBufferData(0x3A, bytes, 1);
            getBytes(timeLimit.getValue(), 1, , bytes);
            _dev.writeBufferData(0x3B, bytes, 1);
            getBytes(timeLatency.getValue(), 10, , bytes);
            _dev.writeBufferData(0x3C, bytes, 10);
            getBytes(timeWindow.getValue(), 1, , bytes);
            _dev.writeBufferData(0x3D, bytes, 1);
            getBytes(activationThreshold.getValue(), 1, , bytes);
            _dev.writeBufferData(0x3E, bytes, 1);
            getBytes(activationDuration.getValue(), 1, , bytes);
            _dev.writeBufferData(0x3F, bytes, 1);
            getBytes(ctrl1.getValue(), 1, , bytes);
            _dev.writeBufferData(0xA0, bytes, 1);
        }

        void update()
        {
            uint8_t bytes[167];
            int i = 0;
            _dev.readBufferData(7, bytes, 167);
            statusRegAux.setValue(getValue(&bytes[i], 1, ));
            i += 1;
            outAdc1.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            outAdc2.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            outAdc3.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            whoAmI.setValue(getValue(&bytes[i], 1, ));
            i += 1;
            fifoSrc.setValue(getValue(&bytes[i], 1, ));
            i += 1;
            int1Src.setValue(getValue(&bytes[i], 1, ));
            i += 1;
            int2Src.setValue(getValue(&bytes[i], 1, ));
            i += 1;
            outX.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            outY.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            outZ.setValue(getValue(&bytes[i], 2, ));
            i += 2;
        }

    };
 }  }  } }