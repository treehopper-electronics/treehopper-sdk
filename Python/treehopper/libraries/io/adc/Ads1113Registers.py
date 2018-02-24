from treehopper.api import *
from treehopper.utils import *
from treehopper.libraries import RegisterManager, Register, SMBusDevice
from treehopper.libraries.Register import sign_extend

class ComparatorQueues:
    AssertAfterOneConversion = 0
    AssertAfterTwoConversions = 1
    AssertAfterFourConversions = 2
    DisableComparator = 3
    
class DataRates:
    Sps_8 = 0
    Sps_16 = 1
    Sps_32 = 2
    Sps_64 = 3
    Sps_128 = 4
    Sps_250 = 5
    Sps_475 = 6
    Sps_860 = 7
    
class Pgas:
    Fsr_6144 = 0
    Fsr_4096 = 1
    Fsr_2048 = 2
    Fsr_1024 = 3
    Fsr_512 = 4
    Fsr_256 = 5
    
class Muxes:
    ain0_ain1 = 0
    ain0_ain3 = 1
    ain1_ain3 = 2
    ain2_ain3 = 3
    ain0_gnd = 4
    ain1_gnd = 5
    ain2_gnd = 6
    ain3_gnd = 7
    
class Ads1113Registers(RegisterManager):
    def __init__(self, dev: SMBusDevice):
        RegisterManager.__init__(self, dev, True)
        self.conversion = self.ConversionRegister(self)
        self.registers.append(self.conversion)
        self.config = self.ConfigRegister(self)
        self.registers.append(self.config)
        self.lowThreshold = self.LowThresholdRegister(self)
        self.registers.append(self.lowThreshold)
        self.highThreshold = self.HighThresholdRegister(self)
        self.registers.append(self.highThreshold)

    class ConversionRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x00, 2, True)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFFFF) << 0)

        def set_value(self, value: int):
            self.value = sign_extend((value >> 0) & 0xFFFF, 16)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 16)\r\n".format(self.value)
            return retVal

    class ConfigRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x01, 2, False)
            self.comparatorQueue = 0
            self.latchingComparator = 0
            self.comparatorPolarity = 0
            self.comparatorMode = 0
            self.dataRate = 0
            self.operatingMode = 0
            self.pga = 0
            self.mux = 0
            self.operationalStatus = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.comparatorQueue & 0x3) << 0) | ((self.latchingComparator & 0x1) << 2) | ((self.comparatorPolarity & 0x1) << 3) | ((self.comparatorMode & 0x1) << 4) | ((self.dataRate & 0x7) << 5) | ((self.operatingMode & 0x1) << 8) | ((self.pga & 0x7) << 9) | ((self.mux & 0x7) << 12) | ((self.operationalStatus & 0x1) << 15)

        def set_value(self, value: int):
            self.comparatorQueue = ((value >> 0) & 0x3)
            self.latchingComparator = ((value >> 2) & 0x1)
            self.comparatorPolarity = ((value >> 3) & 0x1)
            self.comparatorMode = ((value >> 4) & 0x1)
            self.dataRate = ((value >> 5) & 0x7)
            self.operatingMode = ((value >> 8) & 0x1)
            self.pga = ((value >> 9) & 0x7)
            self.mux = ((value >> 12) & 0x7)
            self.operationalStatus = ((value >> 15) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "ComparatorQueue: {} (offset: 0, width: 2)\r\n".format(self.comparatorQueue)
            retVal += "LatchingComparator: {} (offset: 2, width: 1)\r\n".format(self.latchingComparator)
            retVal += "ComparatorPolarity: {} (offset: 3, width: 1)\r\n".format(self.comparatorPolarity)
            retVal += "ComparatorMode: {} (offset: 4, width: 1)\r\n".format(self.comparatorMode)
            retVal += "DataRate: {} (offset: 5, width: 3)\r\n".format(self.dataRate)
            retVal += "OperatingMode: {} (offset: 8, width: 1)\r\n".format(self.operatingMode)
            retVal += "Pga: {} (offset: 9, width: 3)\r\n".format(self.pga)
            retVal += "Mux: {} (offset: 12, width: 3)\r\n".format(self.mux)
            retVal += "OperationalStatus: {} (offset: 15, width: 1)\r\n".format(self.operationalStatus)
            return retVal

    class LowThresholdRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x02, 2, True)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFFFF) << 0)

        def set_value(self, value: int):
            self.value = sign_extend((value >> 0) & 0xFFFF, 16)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 16)\r\n".format(self.value)
            return retVal

    class HighThresholdRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x03, 2, True)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFFFF) << 0)

        def set_value(self, value: int):
            self.value = sign_extend((value >> 0) & 0xFFFF, 16)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 16)\r\n".format(self.value)
            return retVal

