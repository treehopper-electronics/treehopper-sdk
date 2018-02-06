#pragma once
#include "SMBusDevice.h"
#include "Treehopper.Libraries.h"

using namespace Treehopper::Libraries;

namespace Treehopper { namespace Libraries { namespace IO { namespace Adc { 
    class Ads1115Registers
    {
        private:
            SMBusDevice& _dev;

        public:
            Ads1115Registers(SMBusDevice& device) : _dev(device)
            {

            }

            class ConversionRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)(((value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }
            };

            class ConfigRegister
            {
                public:
                int comparatorQueue;
                int latchingComparator;
                int comparatorPolarity;
                int comparatorMode;
                int dataRate;
                int operatingMode;
                int pga;
                int mux;
                int operationalStatus;

                long getValue() { return ((ComparatorQueue & 0x3) << 0) | ((LatchingComparator & 0x1) << 2) | ((ComparatorPolarity & 0x1) << 3) | ((ComparatorMode & 0x1) << 4) | ((DataRate & 0x7) << 5) | ((OperatingMode & 0x1) << 8) | ((Pga & 0x7) << 9) | ((Mux & 0x7) << 12) | ((OperationalStatus & 0x1) << 15); }
                void setValue(long value)
                {
                    comparatorQueue = (int)((value >> 0) & 0x3);
                    latchingComparator = (int)((value >> 2) & 0x1);
                    comparatorPolarity = (int)((value >> 3) & 0x1);
                    comparatorMode = (int)((value >> 4) & 0x1);
                    dataRate = (int)((value >> 5) & 0x7);
                    operatingMode = (int)((value >> 8) & 0x1);
                    pga = (int)((value >> 9) & 0x7);
                    mux = (int)((value >> 12) & 0x7);
                    operationalStatus = (int)((value >> 15) & 0x1);
                }
            };

            class LowThresholdRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)(((value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }
            };

            class HighThresholdRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)(((value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }
            };

            ConversionRegister conversion;
            ConfigRegister config;
            LowThresholdRegister lowThreshold;
            HighThresholdRegister highThreshold;

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
            getBytes(config.getValue(), 2, , bytes);
            _dev.writeBufferData(0x01, bytes, 2);
        }

        void update()
        {
            uint8_t bytes[3];
            int i = 0;
            _dev.readBufferData(2, bytes, 3);
            conversion.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            lowThreshold.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            highThreshold.setValue(getValue(&bytes[i], 2, ));
            i += 2;
        }

    };
 }  }  } }