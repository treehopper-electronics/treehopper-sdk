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
                int OneAxisDataAvailable;
                int TwoAxisDataAvailable;
                int ThreeAxisDataAvailable;
                int DataAvailable;
                int OneAxisDataOverrun;
                int TwoAxisDataOverrun;
                int DataOverrun;

                long getValue() { return ((OneAxisDataAvailable & 0x1) << 0) | ((TwoAxisDataAvailable & 0x1) << 1) | ((ThreeAxisDataAvailable & 0x1) << 2) | ((DataAvailable & 0x1) << 3) | ((OneAxisDataOverrun & 0x1) << 4) | ((TwoAxisDataOverrun & 0x1) << 5) | ((DataOverrun & 0x1) << 6); }
                void setValue(long value)
                {
                    OneAxisDataAvailable = (int)((value >> 0) & 0x1);
                    TwoAxisDataAvailable = (int)((value >> 1) & 0x1);
                    ThreeAxisDataAvailable = (int)((value >> 2) & 0x1);
                    DataAvailable = (int)((value >> 3) & 0x1);
                    OneAxisDataOverrun = (int)((value >> 4) & 0x1);
                    TwoAxisDataOverrun = (int)((value >> 5) & 0x1);
                    DataOverrun = (int)((value >> 6) & 0x1);
                }
            };

            class OutAdc1Register
            {
                public:
                int Value;

                long getValue() { return ((Value & 0x3FF) << 6); }
                void setValue(long value)
                {
                    Value = (int)((value >> 6) & 0x3FF);
                }
            };

            class OutAdc2Register
            {
                public:
                int Value;

                long getValue() { return ((Value & 0x3FF) << 6); }
                void setValue(long value)
                {
                    Value = (int)((value >> 6) & 0x3FF);
                }
            };

            class OutAdc3Register
            {
                public:
                int Value;

                long getValue() { return ((Value & 0x3FF) << 6); }
                void setValue(long value)
                {
                    Value = (int)((value >> 6) & 0x3FF);
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

            class Ctrl0Register
            {
                public:
                int SdoPuDisc;

                long getValue() { return ((SdoPuDisc & 0xFF) << 0); }
                void setValue(long value)
                {
                    SdoPuDisc = (int)((value >> 0) & 0xFF);
                }
            };

            class TempCfgRegRegister
            {
                public:
                int AdcEn;
                int TempEn;

                long getValue() { return ((AdcEn & 0x1) << 7) | ((TempEn & 0x1) << 6); }
                void setValue(long value)
                {
                    AdcEn = (int)((value >> 7) & 0x1);
                    TempEn = (int)((value >> 6) & 0x1);
                }
            };

            class Ctrl2Register
            {
                public:
                int HighPassAoiInt1Enable;
                int HighPassAoiInt2Enable;
                int HighPassClickEnable;
                int FilterDataPassThru;
                int HighPassFilterCutoffFrequency;
                int HighPassFilterModeSelection;

                long getValue() { return ((HighPassAoiInt1Enable & 0x1) << 0) | ((HighPassAoiInt2Enable & 0x1) << 1) | ((HighPassClickEnable & 0x1) << 2) | ((FilterDataPassThru & 0x1) << 3) | ((HighPassFilterCutoffFrequency & 0x3) << 4) | ((HighPassFilterModeSelection & 0x3) << 6); }
                void setValue(long value)
                {
                    HighPassAoiInt1Enable = (int)((value >> 0) & 0x1);
                    HighPassAoiInt2Enable = (int)((value >> 1) & 0x1);
                    HighPassClickEnable = (int)((value >> 2) & 0x1);
                    FilterDataPassThru = (int)((value >> 3) & 0x1);
                    HighPassFilterCutoffFrequency = (int)((value >> 4) & 0x3);
                    HighPassFilterModeSelection = (int)((value >> 6) & 0x3);
                }
            };

            class Ctrl3Register
            {
                public:
                int Overrun;
                int FifoWatermark;
                int Da321;
                int Zyxda;
                int Ia2;
                int Ia1;
                int Click;

                long getValue() { return ((Overrun & 0x1) << 1) | ((FifoWatermark & 0x1) << 2) | ((Da321 & 0x1) << 3) | ((Zyxda & 0x1) << 4) | ((Ia2 & 0x1) << 5) | ((Ia1 & 0x1) << 6) | ((Click & 0x1) << 7); }
                void setValue(long value)
                {
                    Overrun = (int)((value >> 1) & 0x1);
                    FifoWatermark = (int)((value >> 2) & 0x1);
                    Da321 = (int)((value >> 3) & 0x1);
                    Zyxda = (int)((value >> 4) & 0x1);
                    Ia2 = (int)((value >> 5) & 0x1);
                    Ia1 = (int)((value >> 6) & 0x1);
                    Click = (int)((value >> 7) & 0x1);
                }
            };

            class Ctrl4Register
            {
                public:
                int SpiInterfaceMode;
                int SelfTestEnable;
                int HighResolutionOutput;
                int FullScaleSelection;
                int BigEndian;
                int BlockDataUpdate;

                long getValue() { return ((SpiInterfaceMode & 0x1) << 0) | ((SelfTestEnable & 0x3) << 1) | ((HighResolutionOutput & 0x1) << 3) | ((FullScaleSelection & 0x3) << 4) | ((BigEndian & 0x1) << 6) | ((BlockDataUpdate & 0x1) << 7); }
                void setValue(long value)
                {
                    SpiInterfaceMode = (int)((value >> 0) & 0x1);
                    SelfTestEnable = (int)((value >> 1) & 0x3);
                    HighResolutionOutput = (int)((value >> 3) & 0x1);
                    FullScaleSelection = (int)((value >> 4) & 0x3);
                    BigEndian = (int)((value >> 6) & 0x1);
                    BlockDataUpdate = (int)((value >> 7) & 0x1);
                }
            };

            class Ctrl5Register
            {
                public:
                int Enable4DInt2;
                int LatchInt2;
                int Enable4DInt1;
                int LatchInt1;
                int FifoEnable;
                int RebootMemoryContent;

                long getValue() { return ((Enable4DInt2 & 0x1) << 0) | ((LatchInt2 & 0x1) << 1) | ((Enable4DInt1 & 0x1) << 2) | ((LatchInt1 & 0x1) << 3) | ((FifoEnable & 0x1) << 4) | ((RebootMemoryContent & 0x1) << 5); }
                void setValue(long value)
                {
                    Enable4DInt2 = (int)((value >> 0) & 0x1);
                    LatchInt2 = (int)((value >> 1) & 0x1);
                    Enable4DInt1 = (int)((value >> 2) & 0x1);
                    LatchInt1 = (int)((value >> 3) & 0x1);
                    FifoEnable = (int)((value >> 4) & 0x1);
                    RebootMemoryContent = (int)((value >> 5) & 0x1);
                }
            };

            class Ctrl6Register
            {
                public:
                int IntPolarity;
                int Act;
                int Boot;
                int Ia2;
                int Ia1;
                int Click;

                long getValue() { return ((IntPolarity & 0x1) << 1) | ((Act & 0x1) << 3) | ((Boot & 0x1) << 4) | ((Ia2 & 0x1) << 5) | ((Ia1 & 0x1) << 6) | ((Click & 0x1) << 7); }
                void setValue(long value)
                {
                    IntPolarity = (int)((value >> 1) & 0x1);
                    Act = (int)((value >> 3) & 0x1);
                    Boot = (int)((value >> 4) & 0x1);
                    Ia2 = (int)((value >> 5) & 0x1);
                    Ia1 = (int)((value >> 6) & 0x1);
                    Click = (int)((value >> 7) & 0x1);
                }
            };

            class ReferenceRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class StatusRegister
            {
                public:
                int Xda;
                int Yda;
                int Zda;
                int Zyxda;
                int Xor;
                int Yor;
                int Zor;
                int Zyxor;

                long getValue() { return ((Xda & 0x1) << 0) | ((Yda & 0x1) << 1) | ((Zda & 0x1) << 2) | ((Zyxda & 0x1) << 3) | ((Xor & 0x1) << 4) | ((Yor & 0x1) << 5) | ((Zor & 0x1) << 6) | ((Zyxor & 0x1) << 7); }
                void setValue(long value)
                {
                    Xda = (int)((value >> 0) & 0x1);
                    Yda = (int)((value >> 1) & 0x1);
                    Zda = (int)((value >> 2) & 0x1);
                    Zyxda = (int)((value >> 3) & 0x1);
                    Xor = (int)((value >> 4) & 0x1);
                    Yor = (int)((value >> 5) & 0x1);
                    Zor = (int)((value >> 6) & 0x1);
                    Zyxor = (int)((value >> 7) & 0x1);
                }
            };

            class FifoCtrlRegister
            {
                public:
                int FifoThreshold;
                int TriggerSelection;
                int FifoMode;

                long getValue() { return ((FifoThreshold & 0x1F) << 0) | ((TriggerSelection & 0x1) << 5) | ((FifoMode & 0x3) << 6); }
                void setValue(long value)
                {
                    FifoThreshold = (int)((value >> 0) & 0x1F);
                    TriggerSelection = (int)((value >> 5) & 0x1);
                    FifoMode = (int)((value >> 6) & 0x3);
                }
            };

            class FifoSrcRegister
            {
                public:
                int Fss;
                int Emtpy;
                int OverrunFifo;
                int Watermark;

                long getValue() { return ((Fss & 0x1F) << 0) | ((Emtpy & 0x1) << 5) | ((OverrunFifo & 0x1) << 6) | ((Watermark & 0x1) << 7); }
                void setValue(long value)
                {
                    Fss = (int)((value >> 0) & 0x1F);
                    Emtpy = (int)((value >> 5) & 0x1);
                    OverrunFifo = (int)((value >> 6) & 0x1);
                    Watermark = (int)((value >> 7) & 0x1);
                }
            };

            class Int1CfgRegister
            {
                public:
                int EnableXLowEvent;
                int EnableXHighEvent;
                int EnableYLowEvent;
                int EnableYHighEvent;
                int EnableZLowEvent;
                int EnableZHighEvent;
                int Enable6D;
                int AndOrInterruptEvents;

                long getValue() { return ((EnableXLowEvent & 0x1) << 0) | ((EnableXHighEvent & 0x1) << 1) | ((EnableYLowEvent & 0x1) << 2) | ((EnableYHighEvent & 0x1) << 3) | ((EnableZLowEvent & 0x1) << 4) | ((EnableZHighEvent & 0x1) << 5) | ((Enable6D & 0x1) << 6) | ((AndOrInterruptEvents & 0x1) << 7); }
                void setValue(long value)
                {
                    EnableXLowEvent = (int)((value >> 0) & 0x1);
                    EnableXHighEvent = (int)((value >> 1) & 0x1);
                    EnableYLowEvent = (int)((value >> 2) & 0x1);
                    EnableYHighEvent = (int)((value >> 3) & 0x1);
                    EnableZLowEvent = (int)((value >> 4) & 0x1);
                    EnableZHighEvent = (int)((value >> 5) & 0x1);
                    Enable6D = (int)((value >> 6) & 0x1);
                    AndOrInterruptEvents = (int)((value >> 7) & 0x1);
                }
            };

            class Int1SrcRegister
            {
                public:
                int XLow;
                int XHigh;
                int YLow;
                int YHigh;
                int ZLow;
                int ZHigh;
                int InterruptActive;

                long getValue() { return ((XLow & 0x1) << 0) | ((XHigh & 0x1) << 1) | ((YLow & 0x1) << 2) | ((YHigh & 0x1) << 3) | ((ZLow & 0x1) << 4) | ((ZHigh & 0x1) << 5) | ((InterruptActive & 0x1) << 6); }
                void setValue(long value)
                {
                    XLow = (int)((value >> 0) & 0x1);
                    XHigh = (int)((value >> 1) & 0x1);
                    YLow = (int)((value >> 2) & 0x1);
                    YHigh = (int)((value >> 3) & 0x1);
                    ZLow = (int)((value >> 4) & 0x1);
                    ZHigh = (int)((value >> 5) & 0x1);
                    InterruptActive = (int)((value >> 6) & 0x1);
                }
            };

            class Int1ThresholdRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0x7F) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0x7F);
                }
            };

            class Int1DurationRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0x7F) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0x7F);
                }
            };

            class Int2CfgRegister
            {
                public:
                int EnableXLowEvent;
                int EnableXHighEvent;
                int EnableYLowEvent;
                int EnableYHighEvent;
                int EnableZLowEvent;
                int EnableZHighEvent;
                int Enable6D;
                int AndOrInterruptEvents;

                long getValue() { return ((EnableXLowEvent & 0x1) << 0) | ((EnableXHighEvent & 0x1) << 1) | ((EnableYLowEvent & 0x1) << 2) | ((EnableYHighEvent & 0x1) << 3) | ((EnableZLowEvent & 0x1) << 4) | ((EnableZHighEvent & 0x1) << 5) | ((Enable6D & 0x1) << 6) | ((AndOrInterruptEvents & 0x1) << 7); }
                void setValue(long value)
                {
                    EnableXLowEvent = (int)((value >> 0) & 0x1);
                    EnableXHighEvent = (int)((value >> 1) & 0x1);
                    EnableYLowEvent = (int)((value >> 2) & 0x1);
                    EnableYHighEvent = (int)((value >> 3) & 0x1);
                    EnableZLowEvent = (int)((value >> 4) & 0x1);
                    EnableZHighEvent = (int)((value >> 5) & 0x1);
                    Enable6D = (int)((value >> 6) & 0x1);
                    AndOrInterruptEvents = (int)((value >> 7) & 0x1);
                }
            };

            class Int2SrcRegister
            {
                public:
                int XLow;
                int XHigh;
                int YLow;
                int YHigh;
                int ZLow;
                int ZHigh;
                int InterruptActive;

                long getValue() { return ((XLow & 0x1) << 0) | ((XHigh & 0x1) << 1) | ((YLow & 0x1) << 2) | ((YHigh & 0x1) << 3) | ((ZLow & 0x1) << 4) | ((ZHigh & 0x1) << 5) | ((InterruptActive & 0x1) << 6); }
                void setValue(long value)
                {
                    XLow = (int)((value >> 0) & 0x1);
                    XHigh = (int)((value >> 1) & 0x1);
                    YLow = (int)((value >> 2) & 0x1);
                    YHigh = (int)((value >> 3) & 0x1);
                    ZLow = (int)((value >> 4) & 0x1);
                    ZHigh = (int)((value >> 5) & 0x1);
                    InterruptActive = (int)((value >> 6) & 0x1);
                }
            };

            class Int2ThresholdRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0x7F) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0x7F);
                }
            };

            class Int2DurationRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0x7F) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0x7F);
                }
            };

            class ClickSourceRegister
            {
                public:
                int X;
                int Y;
                int Z;
                int Sign;
                int SingleClickEnable;
                int DoubleClickEnable;
                int InterruptActive;

                long getValue() { return ((X & 0x1) << 0) | ((Y & 0x1) << 1) | ((Z & 0x1) << 2) | ((Sign & 0x1) << 3) | ((SingleClickEnable & 0x1) << 4) | ((DoubleClickEnable & 0x1) << 5) | ((InterruptActive & 0x1) << 6); }
                void setValue(long value)
                {
                    X = (int)((value >> 0) & 0x1);
                    Y = (int)((value >> 1) & 0x1);
                    Z = (int)((value >> 2) & 0x1);
                    Sign = (int)((value >> 3) & 0x1);
                    SingleClickEnable = (int)((value >> 4) & 0x1);
                    DoubleClickEnable = (int)((value >> 5) & 0x1);
                    InterruptActive = (int)((value >> 6) & 0x1);
                }
            };

            class ClickThresholdRegister
            {
                public:
                int Threshold;
                int LirClick;

                long getValue() { return ((Threshold & 0x7F) << 0) | ((LirClick & 0x1) << 7); }
                void setValue(long value)
                {
                    Threshold = (int)((value >> 0) & 0x7F);
                    LirClick = (int)((value >> 7) & 0x1);
                }
            };

            class TimeLimitRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0x7F) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0x7F);
                }
            };

            class TimeLatencyRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0x7FFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0x7FFF);
                }
            };

            class TimeWindowRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class ActivationThresholdRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0x7F) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0x7F);
                }
            };

            class ActivationDurationRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFF);
                }
            };

            class Ctrl1Register
            {
                public:
                int XAxisEnable;
                int YAxisEnable;
                int ZAxisEnable;
                int LowPowerEnable;
                int OutputDataRate;

                long getValue() { return ((XAxisEnable & 0x1) << 0) | ((YAxisEnable & 0x1) << 1) | ((ZAxisEnable & 0x1) << 2) | ((LowPowerEnable & 0x1) << 3) | ((OutputDataRate & 0xF) << 4); }
                void setValue(long value)
                {
                    XAxisEnable = (int)((value >> 0) & 0x1);
                    YAxisEnable = (int)((value >> 1) & 0x1);
                    ZAxisEnable = (int)((value >> 2) & 0x1);
                    LowPowerEnable = (int)((value >> 3) & 0x1);
                    OutputDataRate = (int)((value >> 4) & 0xF);
                }
            };

            class OutXRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0x3FF) << 6); }
                void setValue(long value)
                {
                    Value = (int)(((value >> 6) & 0x3FF) << (32 - 6 - 10)) >> (32 - 6 - 10);
                }
            };

            class OutYRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0x3FF) << 6); }
                void setValue(long value)
                {
                    Value = (int)(((value >> 6) & 0x3FF) << (32 - 6 - 10)) >> (32 - 6 - 10);
                }
            };

            class OutZRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0x3FF) << 6); }
                void setValue(long value)
                {
                    Value = (int)(((value >> 6) & 0x3FF) << (32 - 6 - 10)) >> (32 - 6 - 10);
                }
            };

            StatusRegAuxRegister StatusRegAux;
            OutAdc1Register OutAdc1;
            OutAdc2Register OutAdc2;
            OutAdc3Register OutAdc3;
            WhoAmIRegister WhoAmI;
            Ctrl0Register Ctrl0;
            TempCfgRegRegister TempCfgReg;
            Ctrl2Register Ctrl2;
            Ctrl3Register Ctrl3;
            Ctrl4Register Ctrl4;
            Ctrl5Register Ctrl5;
            Ctrl6Register Ctrl6;
            ReferenceRegister Reference;
            StatusRegister Status;
            FifoCtrlRegister FifoCtrl;
            FifoSrcRegister FifoSrc;
            Int1CfgRegister Int1Cfg;
            Int1SrcRegister Int1Src;
            Int1ThresholdRegister Int1Threshold;
            Int1DurationRegister Int1Duration;
            Int2CfgRegister Int2Cfg;
            Int2SrcRegister Int2Src;
            Int2ThresholdRegister Int2Threshold;
            Int2DurationRegister Int2Duration;
            ClickSourceRegister ClickSource;
            ClickThresholdRegister ClickThreshold;
            TimeLimitRegister TimeLimit;
            TimeLatencyRegister TimeLatency;
            TimeWindowRegister TimeWindow;
            ActivationThresholdRegister ActivationThreshold;
            ActivationDurationRegister ActivationDuration;
            Ctrl1Register Ctrl1;
            OutXRegister OutX;
            OutYRegister OutY;
            OutZRegister OutZ;

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
            getBytes(Ctrl0.getValue(), 1, , bytes);
            _dev.writeBufferData(0x1E, bytes, 1);
            getBytes(TempCfgReg.getValue(), 1, , bytes);
            _dev.writeBufferData(0x1F, bytes, 1);
            getBytes(Ctrl2.getValue(), 1, , bytes);
            _dev.writeBufferData(0x21, bytes, 1);
            getBytes(Ctrl3.getValue(), 1, , bytes);
            _dev.writeBufferData(0x22, bytes, 1);
            getBytes(Ctrl4.getValue(), 1, , bytes);
            _dev.writeBufferData(0x23, bytes, 1);
            getBytes(Ctrl5.getValue(), 1, , bytes);
            _dev.writeBufferData(0x24, bytes, 1);
            getBytes(Ctrl6.getValue(), 1, , bytes);
            _dev.writeBufferData(0x25, bytes, 1);
            getBytes(Reference.getValue(), 1, , bytes);
            _dev.writeBufferData(0x26, bytes, 1);
            getBytes(Status.getValue(), 1, , bytes);
            _dev.writeBufferData(0x27, bytes, 1);
            getBytes(FifoCtrl.getValue(), 1, , bytes);
            _dev.writeBufferData(0x2E, bytes, 1);
            getBytes(Int1Cfg.getValue(), 1, , bytes);
            _dev.writeBufferData(0x30, bytes, 1);
            getBytes(Int1Threshold.getValue(), 1, , bytes);
            _dev.writeBufferData(0x32, bytes, 1);
            getBytes(Int1Duration.getValue(), 1, , bytes);
            _dev.writeBufferData(0x33, bytes, 1);
            getBytes(Int2Cfg.getValue(), 1, , bytes);
            _dev.writeBufferData(0x34, bytes, 1);
            getBytes(Int2Threshold.getValue(), 1, , bytes);
            _dev.writeBufferData(0x36, bytes, 1);
            getBytes(Int2Duration.getValue(), 1, , bytes);
            _dev.writeBufferData(0x37, bytes, 1);
            getBytes(ClickSource.getValue(), 1, , bytes);
            _dev.writeBufferData(0x39, bytes, 1);
            getBytes(ClickThreshold.getValue(), 1, , bytes);
            _dev.writeBufferData(0x3A, bytes, 1);
            getBytes(TimeLimit.getValue(), 1, , bytes);
            _dev.writeBufferData(0x3B, bytes, 1);
            getBytes(TimeLatency.getValue(), 10, , bytes);
            _dev.writeBufferData(0x3C, bytes, 10);
            getBytes(TimeWindow.getValue(), 1, , bytes);
            _dev.writeBufferData(0x3D, bytes, 1);
            getBytes(ActivationThreshold.getValue(), 1, , bytes);
            _dev.writeBufferData(0x3E, bytes, 1);
            getBytes(ActivationDuration.getValue(), 1, , bytes);
            _dev.writeBufferData(0x3F, bytes, 1);
            getBytes(Ctrl1.getValue(), 1, , bytes);
            _dev.writeBufferData(0xA0, bytes, 1);
        }

        void update()
        {
            uint8_t bytes[167];
            int i = 0;
            _dev.readBufferData(7, bytes, 167);
            StatusRegAux.setValue(getValue(&bytes[i], 1, ));
            i += 1;
            OutAdc1.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            OutAdc2.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            OutAdc3.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            WhoAmI.setValue(getValue(&bytes[i], 1, ));
            i += 1;
            FifoSrc.setValue(getValue(&bytes[i], 1, ));
            i += 1;
            Int1Src.setValue(getValue(&bytes[i], 1, ));
            i += 1;
            Int2Src.setValue(getValue(&bytes[i], 1, ));
            i += 1;
            OutX.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            OutY.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            OutZ.setValue(getValue(&bytes[i], 2, ));
            i += 2;
        }

    };
 }  }  } }