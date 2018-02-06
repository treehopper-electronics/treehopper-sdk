#pragma once
#include "SMBusDevice.h"
#include "Treehopper.Libraries.h"

using namespace Treehopper::Libraries;

namespace Treehopper { namespace Libraries { {{#NamespaceFragments}}namespace {{.}} { {{/NamespaceFragments}}
    class {{Name}}Registers
    {
        private:
            SMBusDevice& _dev;

        public:
            {{Name}}Registers(SMBusDevice& device) : _dev(device)
            {

            }

            {{#RegisterList}}
            class {{CapitalizedName}}Register
            {
                public:
            {{#Values.Values}}
                int {{Name}};
            {{/Values.Values}}

                long getValue() { return {{#Values.Values}}(({{CapitalizedName}} & {{Bitmask}}) << {{Offset}}){{^Last}} | {{/Last}}{{/Values.Values}}; }
                void setValue(long value)
                {
                    {{#Values.Values}}
                    {{#IsSigned}}
                    {{Name}} = (int)(((value >> {{Offset}}) & {{Bitmask}}) << (32 - {{Width}})) >> (32 - {{Width}});
                    {{/IsSigned}}
                    {{^IsSigned}}
                    {{Name}} = (int)((value >> {{Offset}}) & {{Bitmask}});
                    {{/IsSigned}}
                    {{/Values.Values}}
                }
            };

        {{/RegisterList}}
        {{#RegisterList}}
            {{CapitalizedName}}Register {{Name}};
        {{/RegisterList}}

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
            {{#RegisterList}}
            {{#IsWriteOnly}}
            getBytes({{Name}}.getValue(), {{NumBytes}}, {{LittleEndian}}, bytes);
            _dev.writeBufferData({{Address}}, bytes, {{NumBytes}});
            {{/IsWriteOnly}}
            {{/RegisterList}}
        }

        void update()
        {
            {{#MultiRegisterAccess}}
            uint8_t bytes[{{TotalReadBytes}}];
            int i = 0;
            _dev.readBufferData({{FirstReadAddress}}, bytes, {{TotalReadBytes}});
            {{#RegisterList}}
            {{#IsReadOnly}}
            {{Name}}.setValue(getValue(&bytes[i], {{NumBytes}}, {{LittleEndian}}));
            i += {{NumBytes}};
            {{/IsReadOnly}}
            {{/RegisterList}}
            {{/MultiRegisterAccess}}
            {{^MultiRegisterAccess}}
            {{#RegisterList}}
            {{#IsReadOnly}}
            {{Name}}.setValue(getValue(_dev.readBufferData({{Address}}, {{NumBytes}}), {{LittleEndian}}));
            {{/IsReadOnly}}
            {{/RegisterList}}
            {{/MultiRegisterAccess}}
        }

    };
{{#NamespaceFragments}} } {{/NamespaceFragments}} } }