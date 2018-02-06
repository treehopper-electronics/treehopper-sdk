#pragma once
#include "SMBusDevice.h"
#include "Treehopper.Libraries.h"

using namespace Treehopper::Libraries;

namespace Treehopper { namespace Libraries { namespace Sensors { namespace Magnetic { 
    class Ak8975Registers
    {
        private:
            SMBusDevice& _dev;

        public:
            Ak8975Registers(SMBusDevice& device) : _dev(device)
            {

            }

            class WiaRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class InfoRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class Status1Register
            {
                public:
                int drdy;

                long getValue() { return ((Drdy & 0x1) << 0); }
                void setValue(long value)
                {
                    drdy = (int)((value >> 0) & 0x1);
                }
            };

            class HxRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)(((value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }
            };

            class HyRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)(((value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }
            };

            class HzRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFFFF) << 0); }
                void setValue(long value)
                {
                    value = (int)(((value >> 0) & 0xFFFF) << (32 - 16)) >> (32 - 16);
                }
            };

            class Status2Register
            {
                public:
                int derr;
                int hofl;

                long getValue() { return ((Derr & 0x1) << 2) | ((Hofl & 0x1) << 3); }
                void setValue(long value)
                {
                    derr = (int)((value >> 2) & 0x1);
                    hofl = (int)((value >> 3) & 0x1);
                }
            };

            class ControlRegister
            {
                public:
                int mode;

                long getValue() { return ((Mode & 0xF) << 0); }
                void setValue(long value)
                {
                    mode = (int)((value >> 0) & 0xF);
                }
            };

            class SensitivityXRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class SensitivityYRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            class SensitivityZRegister
            {
                public:
                int value;

                long getValue() { return ((Value & 0xFF) << 0); }
                void setValue(long value)
                {
                    value = (int)((value >> 0) & 0xFF);
                }
            };

            WiaRegister wia;
            InfoRegister info;
            Status1Register status1;
            HxRegister hx;
            HyRegister hy;
            HzRegister hz;
            Status2Register status2;
            ControlRegister control;
            SensitivityXRegister sensitivityX;
            SensitivityYRegister sensitivityY;
            SensitivityZRegister sensitivityZ;

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
        }

        void update()
        {
            uint8_t bytes[0];
            int i = 0;
            _dev.readBufferData(0, bytes, 0);
        }

    };
 }  }  } }