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
                int sleep;
                int measure;

                long getValue() { return ((Sleep & 0x1) << 2) | ((Measure & 0x1) << 3); }
                void setValue(long value)
                {
                    sleep = (int)((value >> 2) & 0x1);
                    measure = (int)((value >> 3) & 0x1);
                }
            };

            class DataFormatRegister
            {
                public:
                int range;
                int justify;
                int fullRes;

                long getValue() { return ((Range & 0x3) << 0) | ((Justify & 0x1) << 2) | ((FullRes & 0x1) << 3); }
                void setValue(long value)
                {
                    range = (int)((value >> 0) & 0x3);
                    justify = (int)((value >> 2) & 0x1);
                    fullRes = (int)((value >> 3) & 0x1);
                }
            };

            class DataXRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0x1FFF) << 0); }
                void setValue(long value)
                {
                    value = (int)(((value >> 0) & 0x1FFF) << (32 - 13)) >> (32 - 13);
                }
            };

            class DataYRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0x1FFF) << 0); }
                void setValue(long value)
                {
                    value = (int)(((value >> 0) & 0x1FFF) << (32 - 13)) >> (32 - 13);
                }
            };

            class DataZRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0x1FFF) << 0); }
                void setValue(long value)
                {
                    value = (int)(((value >> 0) & 0x1FFF) << (32 - 13)) >> (32 - 13);
                }
            };

            PowerCtlRegister powerCtl;
            DataFormatRegister dataFormat;
            DataXRegister dataX;
            DataYRegister dataY;
            DataZRegister dataZ;

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
            getBytes(powerCtl.getValue(), 1, , bytes);
            _dev.writeBufferData(0x2D, bytes, 1);
            getBytes(dataFormat.getValue(), 1, , bytes);
            _dev.writeBufferData(0x31, bytes, 1);
        }

        void update()
        {
            uint8_t bytes[6];
            int i = 0;
            _dev.readBufferData(50, bytes, 6);
            dataX.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            dataY.setValue(getValue(&bytes[i], 2, ));
            i += 2;
            dataZ.setValue(getValue(&bytes[i], 2, ));
            i += 2;
        }

    };
 }  }  } }