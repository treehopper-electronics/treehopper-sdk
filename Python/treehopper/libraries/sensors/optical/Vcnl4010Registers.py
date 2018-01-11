from treehopper.api import *
from treehopper.utils import *
from treehopper.libraries import RegisterManager, Register, SMBusDevice
from treehopper.libraries.Register import sign_extend

class Rates:
    Hz_1_95 = 0
    Hz_3_90625 = 1
    Hz_7_8125 = 2
    Hz_16_625 = 3
    Hz_31_25 = 4
    Hz_62_5 = 5
    Hz_125 = 6
    Hz_250 = 7
    
class AlsRates:
    Hz_1 = 0
    Hz_2 = 1
    Hz_3 = 2
    Hz_4 = 3
    Hz_5 = 4
    Hz_6 = 5
    Hz_8 = 6
    Hz_10 = 7
    
class IntCountExceeds:
    count_1 = 0
    count_2 = 1
    count_4 = 2
    count_8 = 3
    count_16 = 4
    count_32 = 5
    count_64 = 6
    count_128 = 7
    
class Vcnl4010Registers(RegisterManager):
    def __init__(self, dev: SMBusDevice):
        RegisterManager.__init__(self, dev, True)
        self.command = self.CommandRegister(self)
        self.registers.append(self.command)
        self.productId = self.ProductIdRegister(self)
        self.registers.append(self.productId)
        self.proximityRate = self.ProximityRateRegister(self)
        self.registers.append(self.proximityRate)
        self.ledCurrent = self.LedCurrentRegister(self)
        self.registers.append(self.ledCurrent)
        self.ambientLightParameters = self.AmbientLightParametersRegister(self)
        self.registers.append(self.ambientLightParameters)
        self.ambientLightResult = self.AmbientLightResultRegister(self)
        self.registers.append(self.ambientLightResult)
        self.proximityResult = self.ProximityResultRegister(self)
        self.registers.append(self.proximityResult)
        self.interruptControl = self.InterruptControlRegister(self)
        self.registers.append(self.interruptControl)
        self.lowThreshold = self.LowThresholdRegister(self)
        self.registers.append(self.lowThreshold)
        self.highThreshold = self.HighThresholdRegister(self)
        self.registers.append(self.highThreshold)
        self.interruptStatus = self.InterruptStatusRegister(self)
        self.registers.append(self.interruptStatus)
        self.proxModulatorTimingAdustment = self.ProxModulatorTimingAdustmentRegister(self)
        self.registers.append(self.proxModulatorTimingAdustment)

    class CommandRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x80, 1, False)
            self.selfTimedEnable = 0
            self.proxPeriodicEnable = 0
            self.alsPeriodicEnable = 0
            self.proxOnDemandStart = 0
            self.alsOnDemandStart = 0
            self.proxDataReady = 0
            self.alsDataReady = 0
            self.configLock = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.selfTimedEnable & 0x1) << 0) | ((self.proxPeriodicEnable & 0x1) << 1) | ((self.alsPeriodicEnable & 0x1) << 2) | ((self.proxOnDemandStart & 0x1) << 3) | ((self.alsOnDemandStart & 0x1) << 4) | ((self.proxDataReady & 0x1) << 5) | ((self.alsDataReady & 0x1) << 6) | ((self.configLock & 0x1) << 7)

        def set_value(self, value: int):
            self.selfTimedEnable = ((value >> 0) & 0x1)
            self.proxPeriodicEnable = ((value >> 1) & 0x1)
            self.alsPeriodicEnable = ((value >> 2) & 0x1)
            self.proxOnDemandStart = ((value >> 3) & 0x1)
            self.alsOnDemandStart = ((value >> 4) & 0x1)
            self.proxDataReady = ((value >> 5) & 0x1)
            self.alsDataReady = ((value >> 6) & 0x1)
            self.configLock = ((value >> 7) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "SelfTimedEnable: {} (offset: 0, width: 1)\r\n".format(self.selfTimedEnable)
            retVal += "ProxPeriodicEnable: {} (offset: 1, width: 1)\r\n".format(self.proxPeriodicEnable)
            retVal += "AlsPeriodicEnable: {} (offset: 2, width: 1)\r\n".format(self.alsPeriodicEnable)
            retVal += "ProxOnDemandStart: {} (offset: 3, width: 1)\r\n".format(self.proxOnDemandStart)
            retVal += "AlsOnDemandStart: {} (offset: 4, width: 1)\r\n".format(self.alsOnDemandStart)
            retVal += "ProxDataReady: {} (offset: 5, width: 1)\r\n".format(self.proxDataReady)
            retVal += "AlsDataReady: {} (offset: 6, width: 1)\r\n".format(self.alsDataReady)
            retVal += "ConfigLock: {} (offset: 7, width: 1)\r\n".format(self.configLock)
            return retVal

    class ProductIdRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x82, 1, False)
            self.revisionId = 0
            self.productId = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.revisionId & 0xF) << 0) | ((self.productId & 0xF) << 4)

        def set_value(self, value: int):
            self.revisionId = ((value >> 0) & 0xF)
            self.productId = ((value >> 4) & 0xF)

        def __str__(self):
            retVal = ""
            retVal += "RevisionId: {} (offset: 0, width: 4)\r\n".format(self.revisionId)
            retVal += "ProductId: {} (offset: 4, width: 4)\r\n".format(self.productId)
            return retVal

    class ProximityRateRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x82, 1, False)
            self.rate = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.rate & 0xF) << 0)

        def set_value(self, value: int):
            self.rate = ((value >> 0) & 0xF)

        def __str__(self):
            retVal = ""
            retVal += "Rate: {} (offset: 0, width: 4)\r\n".format(self.rate)
            return retVal

    class LedCurrentRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x83, 1, False)
            self.irLedCurrentValue = 0
            self.fuseProgId = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.irLedCurrentValue & 0x3F) << 0) | ((self.fuseProgId & 0x3) << 6)

        def set_value(self, value: int):
            self.irLedCurrentValue = ((value >> 0) & 0x3F)
            self.fuseProgId = ((value >> 6) & 0x3)

        def __str__(self):
            retVal = ""
            retVal += "IrLedCurrentValue: {} (offset: 0, width: 6)\r\n".format(self.irLedCurrentValue)
            retVal += "FuseProgId: {} (offset: 6, width: 2)\r\n".format(self.fuseProgId)
            return retVal

    class AmbientLightParametersRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x84, 1, False)
            self.averagingSamples = 0
            self.autoOffsetCompensation = 0
            self.alsRate = 0
            self.continuousConversionMode = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.averagingSamples & 0x7) << 0) | ((self.autoOffsetCompensation & 0x1) << 3) | ((self.alsRate & 0x7) << 4) | ((self.continuousConversionMode & 0x1) << 7)

        def set_value(self, value: int):
            self.averagingSamples = ((value >> 0) & 0x7)
            self.autoOffsetCompensation = ((value >> 3) & 0x1)
            self.alsRate = ((value >> 4) & 0x7)
            self.continuousConversionMode = ((value >> 7) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "AveragingSamples: {} (offset: 0, width: 3)\r\n".format(self.averagingSamples)
            retVal += "AutoOffsetCompensation: {} (offset: 3, width: 1)\r\n".format(self.autoOffsetCompensation)
            retVal += "AlsRate: {} (offset: 4, width: 3)\r\n".format(self.alsRate)
            retVal += "ContinuousConversionMode: {} (offset: 7, width: 1)\r\n".format(self.continuousConversionMode)
            return retVal

    class AmbientLightResultRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x85, 2, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFFFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFFFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 16)\r\n".format(self.value)
            return retVal

    class ProximityResultRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x87, 2, True)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFFFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFFFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 16)\r\n".format(self.value)
            return retVal

    class InterruptControlRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x89, 1, False)
            self.interruptThresholdSelect = 0
            self.interruptThresholdEnable = 0
            self.interruptAlsReadyEnable = 0
            self.intCountExceed = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.interruptThresholdSelect & 0x1) << 0) | ((self.interruptThresholdEnable & 0x1) << 1) | ((self.interruptAlsReadyEnable & 0x1) << 2) | ((self.intCountExceed & 0x7) << 5)

        def set_value(self, value: int):
            self.interruptThresholdSelect = ((value >> 0) & 0x1)
            self.interruptThresholdEnable = ((value >> 1) & 0x1)
            self.interruptAlsReadyEnable = ((value >> 2) & 0x1)
            self.intCountExceed = ((value >> 5) & 0x7)

        def __str__(self):
            retVal = ""
            retVal += "InterruptThresholdSelect: {} (offset: 0, width: 1)\r\n".format(self.interruptThresholdSelect)
            retVal += "InterruptThresholdEnable: {} (offset: 1, width: 1)\r\n".format(self.interruptThresholdEnable)
            retVal += "InterruptAlsReadyEnable: {} (offset: 2, width: 1)\r\n".format(self.interruptAlsReadyEnable)
            retVal += "IntCountExceed: {} (offset: 5, width: 3)\r\n".format(self.intCountExceed)
            return retVal

    class LowThresholdRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x8A, 2, True)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFFFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFFFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 16)\r\n".format(self.value)
            return retVal

    class HighThresholdRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x8C, 2, True)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFFFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFFFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 16)\r\n".format(self.value)
            return retVal

    class InterruptStatusRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x8E, 1, False)
            self.intThresholdHighExceeded = 0
            self.intThresholdLowExceeded = 0
            self.intAlsReady = 0
            self.intProxReady = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.intThresholdHighExceeded & 0x1) << 0) | ((self.intThresholdLowExceeded & 0x1) << 1) | ((self.intAlsReady & 0x1) << 2) | ((self.intProxReady & 0x1) << 3)

        def set_value(self, value: int):
            self.intThresholdHighExceeded = ((value >> 0) & 0x1)
            self.intThresholdLowExceeded = ((value >> 1) & 0x1)
            self.intAlsReady = ((value >> 2) & 0x1)
            self.intProxReady = ((value >> 3) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "IntThresholdHighExceeded: {} (offset: 0, width: 1)\r\n".format(self.intThresholdHighExceeded)
            retVal += "IntThresholdLowExceeded: {} (offset: 1, width: 1)\r\n".format(self.intThresholdLowExceeded)
            retVal += "IntAlsReady: {} (offset: 2, width: 1)\r\n".format(self.intAlsReady)
            retVal += "IntProxReady: {} (offset: 3, width: 1)\r\n".format(self.intProxReady)
            return retVal

    class ProxModulatorTimingAdustmentRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x8F, 1, False)
            self.modulationDeadTime = 0
            self.proximityFrequency = 0
            self.modulationDelayTime = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.modulationDeadTime & 0x7) << 0) | ((self.proximityFrequency & 0x3) << 3) | ((self.modulationDelayTime & 0x7) << 5)

        def set_value(self, value: int):
            self.modulationDeadTime = ((value >> 0) & 0x7)
            self.proximityFrequency = ((value >> 3) & 0x3)
            self.modulationDelayTime = ((value >> 5) & 0x7)

        def __str__(self):
            retVal = ""
            retVal += "ModulationDeadTime: {} (offset: 0, width: 3)\r\n".format(self.modulationDeadTime)
            retVal += "ProximityFrequency: {} (offset: 3, width: 2)\r\n".format(self.proximityFrequency)
            retVal += "ModulationDelayTime: {} (offset: 5, width: 3)\r\n".format(self.modulationDelayTime)
            return retVal

