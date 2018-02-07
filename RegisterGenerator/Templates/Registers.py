from treehopper.api import *
from treehopper.utils import *
from treehopper.libraries import RegisterManager, Register, SMBusDevice
from treehopper.libraries.Register import sign_extend

{{#RegisterList}}
{{#Values.Values}}
{{#Enum}}
class {{PluralizedName}}:
{{#ValuesList}}
    {{Key}} = {{#Value}}{{Value}}{{/Value}}
{{/ValuesList}}
    
{{/Enum}}
{{/Values.Values}}
{{/RegisterList}}
class {{Name}}Registers(RegisterManager):
    def __init__(self, dev: SMBusDevice):
        RegisterManager.__init__(self, dev, {{MultiRegisterAccessCapitalized}})
        {{#RegisterList}}
        self.{{Name}} = self.{{CapitalizedName}}Register(self)
        self.registers.append(self.{{Name}})
        {{/RegisterList}}

{{#RegisterList}}
    class {{CapitalizedName}}Register(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, {{Address}}, {{NumBytes}}, {{IsBigEndianCapitalized}})
        {{#Values.Values}}
            self.{{Name}} = 0
        {{/Values.Values}}


        def read(self):
            self._manager.read(self)
            return self
            
        def getValue(self):
            return {{#Values.Values}}((self.{{Name}} & {{Bitmask}}) << {{Offset}}){{^Last}} | {{/Last}}{{/Values.Values}}

        def setValue(self, value: int):
            {{#Values.Values}}
            {{#IsSigned}}
            self.{{Name}} = sign_extend((value >> {{Offset}}) & {{Bitmask}}, {{Width}})
            {{/IsSigned}}
            {{^IsSigned}}
            self.{{Name}} = ((value >> {{Offset}}) & {{Bitmask}})
            {{/IsSigned}}
            {{/Values.Values}}

        def __str__(self):
            retVal = ""
            {{#Values.Values}}
            retVal += "{{CapitalizedName}}: {} (offset: {{Offset}}, width: {{Width}})\r\n".format(self.{{Name}})
            {{/Values.Values}}
            return retVal

{{/RegisterList}}
