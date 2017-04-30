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

                long GetValue() { return ((Sleep & 0x1) << 2) | ((Measure & 0x1) << 3); }
                void SetValue(long value)
                {
                    Sleep = (int)((value >> 2) & 0x1);
                    Measure = (int)((value >> 3) & 0x1);
                }
            };

            class DataFormatRegister
            {
                public:
                int Range;

                long GetValue() { return ((Range & 0x3) << 0); }
                void SetValue(long value)
                {
                    Range = (int)((value >> 0) & 0x3);
                }
            };

            class DataXRegister
            {
                public:
                int Value;

                long GetValue() { return ((Value & 0xFFFF) << 0); }
                void SetValue(long value)
                {
                    Value = (int)(((value >> 0) & 0xFFFF) << (32 - 0 - 16)) >> (32 - 0 - 16);
                }
            };

            class DataYRegister
            {
                public:
                int Value;

                long GetValue() { return ((Value & 0xFFFF) << 0); }
                void SetValue(long value)
                {
                    Value = (int)(((value >> 0) & 0xFFFF) << (32 - 0 - 16)) >> (32 - 0 - 16);
                }
            };

            class DataZRegister
            {
                public:
                int Value;

                long GetValue() { return ((Value & 0xFFFF) << 0); }
                void SetValue(long value)
                {
                    Value = (int)(((value >> 0) & 0xFFFF) << (32 - 0 - 16)) >> (32 - 0 - 16);
                }
            };

            PowerCtlRegister PowerCtl;
            DataFormatRegister DataFormat;
            DataXRegister DataX;
            DataYRegister DataY;
            DataZRegister DataZ;

        void GetBytes(long val, int width, bool isLittleEndian, uint8_t* output)
        {
            for (int i = 0; i < width; i++) 
                output[i] = (uint8_t) ((val >> (8 * i)) & 0xFF);

            // TODO: Fix endian
            // if (BitConverter.IsLittleEndian ^ isLittleEndian) 
            //         bytes = bytes.Reverse().ToArray(); 
        }

        long GetValue(uint8_t* bytes, int count, bool isLittleEndian)
        {
            // TODO: Fix endian
            // if (BitConverter.IsLittleEndian ^ isLittleEndian) 
            //         bytes = bytes.Reverse().ToArray(); 
 
            long regVal = 0; 
 
            for (int i = 0; i < count; i++) 
                    regVal |= bytes[i] << (i * 8);

            return regVal;
        }

        void Flush()
        {
            uint8_t bytes[8];
            GetBytes(PowerCtl.GetValue(), 1, true, bytes);
            _dev.writeBufferData(0x2D, bytes, 1);
            GetBytes(DataFormat.GetValue(), 1, true, bytes);
            _dev.writeBufferData(0x31, bytes, 1);
        }

        void Update()
        {
            uint8_t bytes[6];
            int i = 0;
            _dev.readBufferData(50, bytes, 6);
            DataX.SetValue(GetValue(&bytes[i], 2, true));
            i += 2;
            DataY.SetValue(GetValue(&bytes[i], 2, true));
            i += 2;
            DataZ.SetValue(GetValue(&bytes[i], 2, true));
            i += 2;
        }

    };
 }  }  } }