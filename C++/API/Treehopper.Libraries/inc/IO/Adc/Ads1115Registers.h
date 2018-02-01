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
                int Value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)(((value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }
            };

            class ConfigRegister
            {
                public:
                int ComparatorQueue;
                int LatchingComparator;
                int ComparatorPolarity;
                int ComparatorMode;
                int DataRate;
                int OperatingMode;
                int Pga;
                int Mux;
                int OperationalStatus;

                long getValue() { return ((ComparatorQueue & 0x3) << 0) | ((LatchingComparator & 0x1) << 2) | ((ComparatorPolarity & 0x1) << 3) | ((ComparatorMode & 0x1) << 4) | ((DataRate & 0x7) << 5) | ((OperatingMode & 0x1) << 8) | ((Pga & 0x7) << 9) | ((Mux & 0x7) << 12) | ((OperationalStatus & 0x1) << 15); }
                void setValue(long value)
                {
                    ComparatorQueue = (int)((value >> 0) & 0x3);
                    LatchingComparator = (int)((value >> 2) & 0x1);
                    ComparatorPolarity = (int)((value >> 3) & 0x1);
                    ComparatorMode = (int)((value >> 4) & 0x1);
                    DataRate = (int)((value >> 5) & 0x7);
                    OperatingMode = (int)((value >> 8) & 0x1);
                    Pga = (int)((value >> 9) & 0x7);
                    Mux = (int)((value >> 12) & 0x7);
                    OperationalStatus = (int)((value >> 15) & 0x1);
                }
            };

            class LowThresholdRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)(((value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }
            };

            class HighThresholdRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)(((value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }
            };

            ConversionRegister Conversion;
            ConfigRegister Config;
            LowThresholdRegister LowThreshold;
            HighThresholdRegister HighThreshold;

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
            getBytes(Config.getValue(), 2, , bytes);
            _dev.writeBufferData(0x01, bytes, 2);
        }

        void update()
        {
            uint8_t bytes[3];
            int i = 0;
            _dev.readBufferData(2, bytes, 3);
            Conversion.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            LowThreshold.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            HighThreshold.setValue(getValue(&bytes[i], 2, ));
            i += 2;
        }

    };
 }  }  } }