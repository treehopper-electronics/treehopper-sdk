### This file was auto-generated by RegisterGenerator. Any changes to it will be overwritten!
from treehopper.libraries.register_manager_adapter import RegisterManagerAdapter
from treehopper.libraries.register_manager import RegisterManager, Register, sign_extend


class Powers:
    powerDown = 0
    powerUp = 3
    
class IntegrationTimings:
    Time_13_7ms = 0
    Time_101ms = 1
    Time_402ms = 2
    Time_Manual = 3
    
class IntrControlSelects:
    InterruptOutputDisabled = 0
    LevelInterrupt = 1
    SMBAlertCompliant = 2
    TestMode = 3
    
class Tsl2561Registers(RegisterManager):
    def __init__(self, manager: RegisterManagerAdapter):
        RegisterManager.__init__(self, manager, True)
        self.control = self.ControlRegister(self)
        self.registers.append(self.control)
        self.timing = self.TimingRegister(self)
        self.registers.append(self.timing)
        self.interruptThresholdLow = self.InterruptThresholdLowRegister(self)
        self.registers.append(self.interruptThresholdLow)
        self.interruptThresholdHigh = self.InterruptThresholdHighRegister(self)
        self.registers.append(self.interruptThresholdHigh)
        self.interruptControl = self.InterruptControlRegister(self)
        self.registers.append(self.interruptControl)
        self.id = self.IdRegister(self)
        self.registers.append(self.id)
        self.data0 = self.Data0Register(self)
        self.registers.append(self.data0)
        self.data1 = self.Data1Register(self)
        self.registers.append(self.data1)

    class ControlRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x80, 1, False)
            self.power = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def getValue(self):
            return ((self.power & 0x3) << 0)

        def setValue(self, value: int):
            self.power = ((value >> 0) & 0x3)

        def __str__(self):
            retVal = ""
            retVal += "Power: {} (offset: 0, width: 2)\r\n".format(self.power)
            return retVal

    class TimingRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x81, 1, False)
            self.integrationTiming = 0
            self.manual = 0
            self.gain = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def getValue(self):
            return ((self.integrationTiming & 0x3) << 0) | ((self.manual & 0x1) << 3) | ((self.gain & 0x1) << 4)

        def setValue(self, value: int):
            self.integrationTiming = ((value >> 0) & 0x3)
            self.manual = ((value >> 3) & 0x1)
            self.gain = ((value >> 4) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "IntegrationTiming: {} (offset: 0, width: 2)\r\n".format(self.integrationTiming)
            retVal += "Manual: {} (offset: 3, width: 1)\r\n".format(self.manual)
            retVal += "Gain: {} (offset: 4, width: 1)\r\n".format(self.gain)
            return retVal

    class InterruptThresholdLowRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x82, 2, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def getValue(self):
            return ((self.value & 0xFFFF) << 0)

        def setValue(self, value: int):
            self.value = ((value >> 0) & 0xFFFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 16)\r\n".format(self.value)
            return retVal

    class InterruptThresholdHighRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x84, 2, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def getValue(self):
            return ((self.value & 0xFFFF) << 0)

        def setValue(self, value: int):
            self.value = ((value >> 0) & 0xFFFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 16)\r\n".format(self.value)
            return retVal

    class InterruptControlRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x86, 1, False)
            self.persist = 0
            self.intrControlSelect = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def getValue(self):
            return ((self.persist & 0xF) << 0) | ((self.intrControlSelect & 0x3) << 4)

        def setValue(self, value: int):
            self.persist = ((value >> 0) & 0xF)
            self.intrControlSelect = ((value >> 4) & 0x3)

        def __str__(self):
            retVal = ""
            retVal += "Persist: {} (offset: 0, width: 4)\r\n".format(self.persist)
            retVal += "IntrControlSelect: {} (offset: 4, width: 2)\r\n".format(self.intrControlSelect)
            return retVal

    class IdRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x8A, 1, False)
            self.revNumber = 0
            self.partNumber = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def getValue(self):
            return ((self.revNumber & 0xF) << 0) | ((self.partNumber & 0xF) << 4)

        def setValue(self, value: int):
            self.revNumber = ((value >> 0) & 0xF)
            self.partNumber = ((value >> 4) & 0xF)

        def __str__(self):
            retVal = ""
            retVal += "RevNumber: {} (offset: 0, width: 4)\r\n".format(self.revNumber)
            retVal += "PartNumber: {} (offset: 4, width: 4)\r\n".format(self.partNumber)
            return retVal

    class Data0Register(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x8C, 2, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def getValue(self):
            return ((self.value & 0xFFFF) << 0)

        def setValue(self, value: int):
            self.value = ((value >> 0) & 0xFFFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 16)\r\n".format(self.value)
            return retVal

    class Data1Register(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x8E, 2, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def getValue(self):
            return ((self.value & 0xFFFF) << 0)

        def setValue(self, value: int):
            self.value = ((value >> 0) & 0xFFFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 16)\r\n".format(self.value)
            return retVal

