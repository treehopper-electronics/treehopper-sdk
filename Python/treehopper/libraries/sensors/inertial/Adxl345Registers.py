from treehopper.api import *
from treehopper.utils import *
from treehopper.libraries import RegisterManager, Register, SMBusDevice
from treehopper.libraries.Register import sign_extend

class Adxl345Registers(RegisterManager):
    def __init__(self, dev: SMBusDevice):
        RegisterManager.__init__(self, dev, True)
        self.powerCtl = self.PowerCtlRegister(self)
        self.registers.append(self.powerCtl)
        self.dataFormat = self.DataFormatRegister(self)
        self.registers.append(self.dataFormat)
        self.dataX = self.DataXRegister(self)
        self.registers.append(self.dataX)
        self.dataY = self.DataYRegister(self)
        self.registers.append(self.dataY)
        self.dataZ = self.DataZRegister(self)
        self.registers.append(self.dataZ)

    class PowerCtlRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x2D, 1, False)
            self.sleep = 0
            self.measure = 0

        def read(self):
            self.manager.read(self)
            return self
            
        def get_value(self):
            return ((self.sleep & 0x1) << 2) | ((self.measure & 0x1) << 3)

        def set_value(self, value: int):
            self.sleep = ((value >> 2) & 0x1)
            self.measure = ((value >> 3) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "Sleep: {} (offset: 2, width: 1)\r\n".format(self.sleep)
            retVal += "Measure: {} (offset: 3, width: 1)\r\n".format(self.measure)
            return retVal

    class DataFormatRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x31, 1, False)
            self.range = 0
            self.justify = 0
            self.fullRes = 0

        def read(self):
            self.manager.read(self)
            return self
            
        def get_value(self):
            return ((self.range & 0x3) << 0) | ((self.justify & 0x1) << 2) | ((self.fullRes & 0x1) << 3)

        def set_value(self, value: int):
            self.range = ((value >> 0) & 0x3)
            self.justify = ((value >> 2) & 0x1)
            self.fullRes = ((value >> 3) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "Range: {} (offset: 0, width: 2)\r\n".format(self.range)
            retVal += "Justify: {} (offset: 2, width: 1)\r\n".format(self.justify)
            retVal += "FullRes: {} (offset: 3, width: 1)\r\n".format(self.fullRes)
            return retVal

    class DataXRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x32, 2, False)
            self.value = 0

        def read(self):
            self.manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0x1FFF) << 0)

        def set_value(self, value: int):
            self.value = sign_extend((value >> 0) & 0x1FFF, 13)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 13)\r\n".format(self.value)
            return retVal

    class DataYRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x34, 2, False)
            self.value = 0

        def read(self):
            self.manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0x1FFF) << 0)

        def set_value(self, value: int):
            self.value = sign_extend((value >> 0) & 0x1FFF, 13)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 13)\r\n".format(self.value)
            return retVal

    class DataZRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x36, 2, False)
            self.value = 0

        def read(self):
            self.manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0x1FFF) << 0)

        def set_value(self, value: int):
            self.value = sign_extend((value >> 0) & 0x1FFF, 13)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 13)\r\n".format(self.value)
            return retVal

