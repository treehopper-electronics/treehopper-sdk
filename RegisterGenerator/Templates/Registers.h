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

            {{#Registers}}
            class {{CapitalizedName}}Register
            {
                public:
            {{#Values}}
                int {{CapitalizedName}};
            {{/Values}}

                long GetValue() { return {{#Values}}(({{CapitalizedName}} & {{Bitmask}}) << {{Offset}}){{^Last}} | {{/Last}}{{/Values}}; }
                void SetValue(long value)
                {
                    {{#Values}}
                    {{#IsSigned}}
                    {{CapitalizedName}} = (int)(((value >> {{Offset}}) & {{Bitmask}}) << (32 - {{Offset}} - {{Width}})) >> (32 - {{Offset}} - {{Width}});
                    {{/IsSigned}}
                    {{^IsSigned}}
                    {{CapitalizedName}} = (int)((value >> {{Offset}}) & {{Bitmask}});
                    {{/IsSigned}}
                    {{/Values}}
                }
            };

        {{/Registers}}
        {{#Registers}}
            {{CapitalizedName}}Register {{CapitalizedName}};
        {{/Registers}}

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
            {{#Registers}}
            {{#IsWriteOnly}}
            GetBytes({{CapitalizedName}}.GetValue(), {{NumBytes}}, {{LittleEndian}}, bytes);
            _dev.writeBufferData({{Address}}, bytes, {{NumBytes}});
            {{/IsWriteOnly}}
            {{/Registers}}
        }

        void Update()
        {
            {{#MultiRegisterAccess}}
            uint8_t bytes[{{TotalReadBytes}}];
            int i = 0;
            _dev.readBufferData({{FirstReadAddress}}, bytes, {{TotalReadBytes}});
            {{#Registers}}
            {{#IsReadOnly}}
            {{CapitalizedName}}.SetValue(GetValue(&bytes[i], {{NumBytes}}, {{LittleEndian}}));
            i += {{NumBytes}};
            {{/IsReadOnly}}
            {{/Registers}}
            {{/MultiRegisterAccess}}
            {{^MultiRegisterAccess}}
            {{#Registers}}
            {{#IsReadOnly}}
            {{CapitalizedName}}.SetValue(GetValue(_dev.readBufferData({{Address}}, {{NumBytes}}), {{LittleEndian}}));
            {{/IsReadOnly}}
            {{/Registers}}
            {{/MultiRegisterAccess}}
        }

    };
{{#NamespaceFragments}} } {{/NamespaceFragments}} } }