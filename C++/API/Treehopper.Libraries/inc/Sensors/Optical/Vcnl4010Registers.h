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
                int selfTimedEnable;
                int proxPeriodicEnable;
                int alsPeriodicEnable;
                int proxOnDemandStart;
                int alsOnDemandStart;
                int proxDataReady;
                int alsDataReady;
                int configLock;

                long getValue() { return ((SelfTimedEnable & 0x1) << 0) | ((ProxPeriodicEnable & 0x1) << 1) | ((AlsPeriodicEnable & 0x1) << 2) | ((ProxOnDemandStart & 0x1) << 3) | ((AlsOnDemandStart & 0x1) << 4) | ((ProxDataReady & 0x1) << 5) | ((AlsDataReady & 0x1) << 6) | ((ConfigLock & 0x1) << 7); }
                void setValue(long value)
                {
                    selfTimedEnable = (int)((value >> 0) & 0x1);
                    proxPeriodicEnable = (int)((value >> 1) & 0x1);
                    alsPeriodicEnable = (int)((value >> 2) & 0x1);
                    proxOnDemandStart = (int)((value >> 3) & 0x1);
                    alsOnDemandStart = (int)((value >> 4) & 0x1);
                    proxDataReady = (int)((value >> 5) & 0x1);
                    alsDataReady = (int)((value >> 6) & 0x1);
                    configLock = (int)((value >> 7) & 0x1);
                }
            };

            class ProductIdRegister
            {
                public:
                int revisionId;
                int productId;

                long getValue() { return ((RevisionId & 0xF) << 0) | ((ProductId & 0xF) << 4); }
                void setValue(long value)
                {
                    revisionId = (int)((value >> 0) & 0xF);
                    productId = (int)((value >> 4) & 0xF);
                }
            };

            class ProximityRateRegister
            {
                public:
                int rate;

                long getValue() { return ((Rate & 0xF) << 0); }
                void setValue(long value)
                {
                    rate = (int)((value >> 0) & 0xF);
                }
            };

            class LedCurrentRegister
            {
                public:
                int irLedCurrentValue;
                int fuseProgId;

                long getValue() { return ((IrLedCurrentValue & 0x3F) << 0) | ((FuseProgId & 0x3) << 6); }
                void setValue(long value)
                {
                    irLedCurrentValue = (int)((value >> 0) & 0x3F);
                    fuseProgId = (int)((value >> 6) & 0x3);
                }
            };

            class AmbientLightParametersRegister
            {
                public:
                int averagingSamples;
                int autoOffsetCompensation;
                int alsRate;
                int continuousConversionMode;

                long getValue() { return ((AveragingSamples & 0x7) << 0) | ((AutoOffsetCompensation & 0x1) << 3) | ((AlsRate & 0x7) << 4) | ((ContinuousConversionMode & 0x1) << 7); }
                void setValue(long value)
                {
                    averagingSamples = (int)((value >> 0) & 0x7);
                    autoOffsetCompensation = (int)((value >> 3) & 0x1);
                    alsRate = (int)((value >> 4) & 0x7);
                    continuousConversionMode = (int)((value >> 7) & 0x1);
                }
            };

            class AmbientLightResultRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFFFF);
                }
            };

            class ProximityResultRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFFFF);
                }
            };

            class InterruptControlRegister
            {
                public:
                int interruptThresholdSelect;
                int interruptThresholdEnable;
                int interruptAlsReadyEnable;
                int intCountExceed;

                long getValue() { return ((InterruptThresholdSelect & 0x1) << 0) | ((InterruptThresholdEnable & 0x1) << 1) | ((InterruptAlsReadyEnable & 0x1) << 2) | ((IntCountExceed & 0x7) << 5); }
                void setValue(long value)
                {
                    interruptThresholdSelect = (int)((value >> 0) & 0x1);
                    interruptThresholdEnable = (int)((value >> 1) & 0x1);
                    interruptAlsReadyEnable = (int)((value >> 2) & 0x1);
                    intCountExceed = (int)((value >> 5) & 0x7);
                }
            };

            class LowThresholdRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFFFF);
                }
            };

            class HighThresholdRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFFFF);
                }
            };

            class InterruptStatusRegister
            {
                public:
                int intThresholdHighExceeded;
                int intThresholdLowExceeded;
                int intAlsReady;
                int intProxReady;

                long getValue() { return ((IntThresholdHighExceeded & 0x1) << 0) | ((IntThresholdLowExceeded & 0x1) << 1) | ((IntAlsReady & 0x1) << 2) | ((IntProxReady & 0x1) << 3); }
                void setValue(long value)
                {
                    intThresholdHighExceeded = (int)((value >> 0) & 0x1);
                    intThresholdLowExceeded = (int)((value >> 1) & 0x1);
                    intAlsReady = (int)((value >> 2) & 0x1);
                    intProxReady = (int)((value >> 3) & 0x1);
                }
            };

            class ProxModulatorTimingAdustmentRegister
            {
                public:
                int modulationDeadTime;
                int proximityFrequency;
                int modulationDelayTime;

                long getValue() { return ((ModulationDeadTime & 0x7) << 0) | ((ProximityFrequency & 0x3) << 3) | ((ModulationDelayTime & 0x7) << 5); }
                void setValue(long value)
                {
                    modulationDeadTime = (int)((value >> 0) & 0x7);
                    proximityFrequency = (int)((value >> 3) & 0x3);
                    modulationDelayTime = (int)((value >> 5) & 0x7);
                }
            };

            CommandRegister command;
            ProductIdRegister productId;
            ProximityRateRegister proximityRate;
            LedCurrentRegister ledCurrent;
            AmbientLightParametersRegister ambientLightParameters;
            AmbientLightResultRegister ambientLightResult;
            ProximityResultRegister proximityResult;
            InterruptControlRegister interruptControl;
            LowThresholdRegister lowThreshold;
            HighThresholdRegister highThreshold;
            InterruptStatusRegister interruptStatus;
            ProxModulatorTimingAdustmentRegister proxModulatorTimingAdustment;

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
            getBytes(command.getValue(), 1, , bytes);
            _dev.writeBufferData(0x80, bytes, 1);
            getBytes(proximityRate.getValue(), 1, , bytes);
            _dev.writeBufferData(0x82, bytes, 1);
            getBytes(ledCurrent.getValue(), 1, , bytes);
            _dev.writeBufferData(0x83, bytes, 1);
            getBytes(ambientLightParameters.getValue(), 1, , bytes);
            _dev.writeBufferData(0x84, bytes, 1);
        }

        void update()
        {
            uint8_t bytes[7];
            int i = 0;
            _dev.readBufferData(130, bytes, 7);
            productId.setValue(getValue(&bytes[i], 1, ));
            i += 1;
            ambientLightResult.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            proximityResult.setValue(getValue(&bytes[i], 2, ));
            i += 2;
        }

    };
 }  }  } }