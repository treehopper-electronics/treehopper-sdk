#pragma once
#include "SMBusDevice.h"
#include "Treehopper.Libraries.h"

using namespace Treehopper::Libraries;

namespace Treehopper { namespace Libraries { namespace Sensors { namespace Inertial { 
    class Adxl345Registers
    {
        private:
            SMBusDevice& _dev;

        public:
            Adxl345Registers(SMBusDevice& device) : _dev(device)
            {

            }

            class PowerCtlRegister
            {
                public:
                int Sleep;
                int Measure;

                long getValue() { return ((Sleep & 0x1) << 2) | ((Measure & 0x1) << 3); }
                void setValue(long value)
                {
                    Sleep = (int)((value >> 2) & 0x1);
                    Measure = (int)((value >> 3) & 0x1);
                }
            };

            class DataFormatRegister
            {
                public:
                int Range;
                int Justify;
                int FullRes;

                long getValue() { return ((Range & 0x3) << 0) | ((Justify & 0x1) << 2) | ((FullRes & 0x1) << 3); }
                void setValue(long value)
                {
                    Range = (int)((value >> 0) & 0x3);
                    Justify = (int)((value >> 2) & 0x1);
                    FullRes = (int)((value >> 3) & 0x1);
                }
            };

            class DataXRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0x1FFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)(((value >> 0) & 0x1FFF) << (32 - 13)) >> (32 - 13);
                }
            };

            class DataYRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0x1FFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)(((value >> 0) & 0x1FFF) << (32 - 13)) >> (32 - 13);
                }
            };

            class DataZRegister
            {
                public:
                int Value;

                long getValue() { return ((Value & 0x1FFF) << 0); }
                void setValue(long value)
                {
                    Value = (int)(((value >> 0) & 0x1FFF) << (32 - 13)) >> (32 - 13);
                }
            };

            PowerCtlRegister PowerCtl;
            DataFormatRegister DataFormat;
            DataXRegister DataX;
            DataYRegister DataY;
            DataZRegister DataZ;

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
            getBytes(PowerCtl.getValue(), 1, , bytes);
            _dev.writeBufferData(0x2D, bytes, 1);
            getBytes(DataFormat.getValue(), 1, , bytes);
            _dev.writeBufferData(0x31, bytes, 1);
        }

        void update()
        {
            uint8_t bytes[6];
            int i = 0;
            _dev.readBufferData(50, bytes, 6);
            DataX.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            DataY.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            DataZ.setValue(getValue(&bytes[i], 2, ));
            i += 2;
        }

    };
 }  }  } }