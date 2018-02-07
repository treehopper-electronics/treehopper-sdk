#pragma once
#include "SMBusDevice.h"
#include "Treehopper.Libraries.h"

using namespace Treehopper::Libraries;

namespace Treehopper { namespace Libraries { namespace Sensors { namespace Optical { 
    class Tsl2591Registers
    {
        private:
            SMBusDevice& _dev;

        public:
            Tsl2591Registers(SMBusDevice& device) : _dev(device)
            {

            }

            class EnableRegister
            {
                public:
                int powerOn;
                int alsEnable;
                int alsInterruptEnable;
                int sleepAfterInterrupt;
                int noPersistInterruptEnable;

                long getValue() { return ((PowerOn & 0x1) << 0) | ((AlsEnable & 0x1) << 1) | ((AlsInterruptEnable & 0x1) << 4) | ((SleepAfterInterrupt & 0x1) << 6) | ((NoPersistInterruptEnable & 0x1) << 7); }
                void setValue(long value)
                {
                    powerOn = (int)((value >> 0) & 0x1);
                    alsEnable = (int)((value >> 1) & 0x1);
                    alsInterruptEnable = (int)((value >> 4) & 0x1);
                    sleepAfterInterrupt = (int)((value >> 6) & 0x1);
                    noPersistInterruptEnable = (int)((value >> 7) & 0x1);
                }
            };

            class ConfigRegister
            {
                public:
                int alsTime;
                int alsGain;
                int systemReset;

                long getValue() { return ((AlsTime & 0x7) << 0) | ((AlsGain & 0x3) << 3) | ((SystemReset & 0x1) << 7); }
                void setValue(long value)
                {
                    alsTime = (int)((value >> 0) & 0x7);
                    alsGain = (int)((value >> 3) & 0x3);
                    systemReset = (int)((value >> 7) & 0x1);
                }
            };

            class InterruptLowThresholdRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFFFF);
                }
            };

            class InterruptHighThresholdRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFFFF);
                }
            };

            class NoPersistLowThresholdRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFFFF);
                }
            };

            class NoPersistHighThresholdRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFFFF);
                }
            };

            class PersistRegister
            {
                public:
                int interruptPersistanceFilter;

                long getValue() { return ((InterruptPersistanceFilter & 0xF) << 0); }
                void setValue(long value)
                {
                    interruptPersistanceFilter = (int)((value >> 0) & 0xF);
                }
            };

            class PackageIdRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class DeviceIdRegister
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
                int alsValud;
                int alsInterrupt;
                int noPersistInterrupt;

                long getValue() { return ((AlsValud & 0x1) << 0) | ((AlsInterrupt & 0x1) << 4) | ((NoPersistInterrupt & 0x1) << 5); }
                void setValue(long value)
                {
                    alsValud = (int)((value >> 0) & 0x1);
                    alsInterrupt = (int)((value >> 4) & 0x1);
                    noPersistInterrupt = (int)((value >> 5) & 0x1);
                }
            };

            class Ch0Register
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFFFF);
                }
            };

            class Ch1Register
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFFFF);
                }
            };

            EnableRegister enable;
            ConfigRegister config;
            InterruptLowThresholdRegister interruptLowThreshold;
            InterruptHighThresholdRegister interruptHighThreshold;
            NoPersistLowThresholdRegister noPersistLowThreshold;
            NoPersistHighThresholdRegister noPersistHighThreshold;
            PersistRegister persist;
            PackageIdRegister packageId;
            DeviceIdRegister deviceId;
            StatusRegister status;
            Ch0Register ch0;
            Ch1Register ch1;

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
            getBytes(enable.getValue(), 1, , bytes);
            _dev.writeBufferData(0xA0, bytes, 1);
            getBytes(config.getValue(), 1, , bytes);
            _dev.writeBufferData(0xA1, bytes, 1);
            getBytes(interruptLowThreshold.getValue(), 2, , bytes);
            _dev.writeBufferData(0xA4, bytes, 2);
            getBytes(interruptHighThreshold.getValue(), 2, , bytes);
            _dev.writeBufferData(0xA6, bytes, 2);
            getBytes(noPersistLowThreshold.getValue(), 2, , bytes);
            _dev.writeBufferData(0xA8, bytes, 2);
            getBytes(noPersistHighThreshold.getValue(), 2, , bytes);
            _dev.writeBufferData(0xAa, bytes, 2);
            getBytes(persist.getValue(), 1, , bytes);
            _dev.writeBufferData(0xAc, bytes, 1);
        }

        void update()
        {
            uint8_t bytes[7];
            int i = 0;
            _dev.readBufferData(177, bytes, 7);
            packageId.setValue(getValue(&bytes[i], 1, ));
            i += 1;
            deviceId.setValue(getValue(&bytes[i], 1, ));
            i += 1;
            status.setValue(getValue(&bytes[i], 1, ));
            i += 1;
            ch0.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            ch1.setValue(getValue(&bytes[i], 2, ));
            i += 2;
        }

    };
 }  }  } }