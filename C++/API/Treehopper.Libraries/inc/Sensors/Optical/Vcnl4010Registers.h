#pragma once
#include "SMBusDevice.h"
#include "Treehopper.Libraries.h"

using namespace Treehopper::Libraries;

namespace Treehopper { namespace Libraries { namespace Sensors { namespace Optical { 
    class Vcnl4010Registers
    {
        private:
            SMBusDevice& _dev;

        public:
            Vcnl4010Registers(SMBusDevice& device) : _dev(device)
            {

            }

            class CommandRegister
            {
                public:
                int SelfTimedEnable;
                int ProxPeriodicEnable;
                int AlsPeriodicEnable;
                int ProxOnDemandStart;
                int AlsOnDemandStart;
                int ProxDataReady;
                int AlsDataReady;
                int ConfigLock;

                long getValue() { return ((SelfTimedEnable & 0x1) << 0) | ((ProxPeriodicEnable & 0x1) << 1) | ((AlsPeriodicEnable & 0x1) << 2) | ((ProxOnDemandStart & 0x1) << 3) | ((AlsOnDemandStart & 0x1) << 4) | ((ProxDataReady & 0x1) << 5) | ((AlsDataReady & 0x1) << 6) | ((ConfigLock & 0x1) << 7); }
                void setValue(long value)
                {
                    SelfTimedEnable = (int)((value >> 0) & 0x1);
                    ProxPeriodicEnable = (int)((value >> 1) & 0x1);
                    AlsPeriodicEnable = (int)((value >> 2) & 0x1);
                    ProxOnDemandStart = (int)((value >> 3) & 0x1);
                    AlsOnDemandStart = (int)((value >> 4) & 0x1);
                    ProxDataReady = (int)((value >> 5) & 0x1);
                    AlsDataReady = (int)((value >> 6) & 0x1);
                    ConfigLock = (int)((value >> 7) & 0x1);
                }
            };

            class ProductIdRegister
            {
                public:
                int RevisionId;
                int ProductId;

                long getValue() { return ((RevisionId & 0xF) << 0) | ((ProductId & 0xF) << 4); }
                void setValue(long value)
                {
                    RevisionId = (int)((value >> 0) & 0xF);
                    ProductId = (int)((value >> 4) & 0xF);
                }
            };

            class ProximityRateRegister
            {
                public:
                int Rate;

                long getValue() { return ((Rate & 0xF) << 0); }
                void setValue(long value)
                {
                    Rate = (int)((value >> 0) & 0xF);
                }
            };

            class LedCurrentRegister
            {
                public:
                int IrLedCurrentValue;
                int FuseProgId;

                long getValue() { return ((IrLedCurrentValue & 0x3F) << 0) | ((FuseProgId & 0x3) << 6); }
                void setValue(long value)
                {
                    IrLedCurrentValue = (int)((value >> 0) & 0x3F);
                    FuseProgId = (int)((value >> 6) & 0x3);
                }
            };

            class AmbientLightParametersRegister
            {
                public:
                int AveragingSamples;
                int AutoOffsetCompensation;
                int AlsRate;
                int ContinuousConversionMode;

                long getValue() { return ((AveragingSamples & 0x7) << 0) | ((AutoOffsetCompensation & 0x1) << 3) | ((AlsRate & 0x7) << 4) | ((ContinuousConversionMode & 0x1) << 7); }
                void setValue(long value)
                {
                    AveragingSamples = (int)((value >> 0) & 0x7);
                    AutoOffsetCompensation = (int)((value >> 3) & 0x1);
                    AlsRate = (int)((value >> 4) & 0x7);
                    ContinuousConversionMode = (int)((value >> 7) & 0x1);
                }
            };

            class AmbientLightResultRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFFFF);
                }
            };

            class ProximityResultRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFFFF);
                }
            };

            class InterruptControlRegister
            {
                public:
                int InterruptThresholdSelect;
                int InterruptThresholdEnable;
                int InterruptAlsReadyEnable;
                int IntCountExceed;

                long getValue() { return ((InterruptThresholdSelect & 0x1) << 0) | ((InterruptThresholdEnable & 0x1) << 1) | ((InterruptAlsReadyEnable & 0x1) << 2) | ((IntCountExceed & 0x7) << 5); }
                void setValue(long value)
                {
                    InterruptThresholdSelect = (int)((value >> 0) & 0x1);
                    InterruptThresholdEnable = (int)((value >> 1) & 0x1);
                    InterruptAlsReadyEnable = (int)((value >> 2) & 0x1);
                    IntCountExceed = (int)((value >> 5) & 0x7);
                }
            };

            class LowThresholdRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFFFF);
                }
            };

            class HighThresholdRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)((value >> 0) & 0xFFFF);
                }
            };

            class InterruptStatusRegister
            {
                public:
                int IntThresholdHighExceeded;
                int IntThresholdLowExceeded;
                int IntAlsReady;
                int IntProxReady;

                long getValue() { return ((IntThresholdHighExceeded & 0x1) << 0) | ((IntThresholdLowExceeded & 0x1) << 1) | ((IntAlsReady & 0x1) << 2) | ((IntProxReady & 0x1) << 3); }
                void setValue(long value)
                {
                    IntThresholdHighExceeded = (int)((value >> 0) & 0x1);
                    IntThresholdLowExceeded = (int)((value >> 1) & 0x1);
                    IntAlsReady = (int)((value >> 2) & 0x1);
                    IntProxReady = (int)((value >> 3) & 0x1);
                }
            };

            class ProxModulatorTimingAdustmentRegister
            {
                public:
                int ModulationDeadTime;
                int ProximityFrequency;
                int ModulationDelayTime;

                long getValue() { return ((ModulationDeadTime & 0x7) << 0) | ((ProximityFrequency & 0x3) << 3) | ((ModulationDelayTime & 0x7) << 5); }
                void setValue(long value)
                {
                    ModulationDeadTime = (int)((value >> 0) & 0x7);
                    ProximityFrequency = (int)((value >> 3) & 0x3);
                    ModulationDelayTime = (int)((value >> 5) & 0x7);
                }
            };

            CommandRegister Command;
            ProductIdRegister ProductId;
            ProximityRateRegister ProximityRate;
            LedCurrentRegister LedCurrent;
            AmbientLightParametersRegister AmbientLightParameters;
            AmbientLightResultRegister AmbientLightResult;
            ProximityResultRegister ProximityResult;
            InterruptControlRegister InterruptControl;
            LowThresholdRegister LowThreshold;
            HighThresholdRegister HighThreshold;
            InterruptStatusRegister InterruptStatus;
            ProxModulatorTimingAdustmentRegister ProxModulatorTimingAdustment;

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
            getBytes(Command.getValue(), 1, , bytes);
            _dev.writeBufferData(0x80, bytes, 1);
            getBytes(ProximityRate.getValue(), 1, , bytes);
            _dev.writeBufferData(0x82, bytes, 1);
            getBytes(LedCurrent.getValue(), 1, , bytes);
            _dev.writeBufferData(0x83, bytes, 1);
            getBytes(AmbientLightParameters.getValue(), 1, , bytes);
            _dev.writeBufferData(0x84, bytes, 1);
        }

        void update()
        {
            uint8_t bytes[7];
            int i = 0;
            _dev.readBufferData(130, bytes, 7);
            ProductId.setValue(getValue(&bytes[i], 1, ));
            i += 1;
            AmbientLightResult.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            ProximityResult.setValue(getValue(&bytes[i], 2, ));
            i += 2;
        }

    };
 }  }  } }