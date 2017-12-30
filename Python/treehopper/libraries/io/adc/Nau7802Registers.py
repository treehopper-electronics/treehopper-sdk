from treehopper.api import *
from treehopper.utils import *
from treehopper.libraries import RegisterManager, Register, SMBusDevice
from treehopper.libraries.Register import sign_extend

class Gains:
    x1 = 0
    x4 = 1
    x2 = 2
    x8 = 3
    x16 = 4
    x32 = 5
    x64 = 6
    x128 = 7
    
class LdoVoltage:
    mV_4500 = 0
    mV_4200 = 1
    mV_3900 = 2
    mV_3600 = 3
    mV_3300 = 4
    mV_3000 = 5
    mV_2700 = 6
    mV_2400 = 7
    
class CalMods:
    OffsetCalibrationInternal = 0
    Reserved = 1
    OffsetCalibrationSystem = 2
    GainCalibrationSystem = 3
    
class ConversionRates:
    Sps_10 = 0
    Sps_20 = 1
    Sps_40 = 2
    Sps_80 = 3
    Sps_320 = 7
    
class AdcVcms:
    ExtendedCommonModeRefp = 3
    ExtendedCommonModeRefn = 2
    disable = 0
    
class RegChpFreqs:
    off = 3
    
class Nau7802Registers(RegisterManager):
    def __init__(self, dev: SMBusDevice):
        RegisterManager.__init__(self, dev, True)
        self.puCtrl = self.PuCtrlRegister(self)
        self.registers.append(self.puCtrl)
        self.ctrl1 = self.Ctrl1Register(self)
        self.registers.append(self.ctrl1)
        self.ctrl2 = self.Ctrl2Register(self)
        self.registers.append(self.ctrl2)
        self.i2cCtrl = self.I2cCtrlRegister(self)
        self.registers.append(self.i2cCtrl)
        self.adcResult = self.AdcResultRegister(self)
        self.registers.append(self.adcResult)
        self.adc = self.AdcRegister(self)
        self.registers.append(self.adc)
        self.pga = self.PgaRegister(self)
        self.registers.append(self.pga)
        self.powerCtrl = self.PowerCtrlRegister(self)
        self.registers.append(self.powerCtrl)

    class PuCtrlRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x00, 1, False)
            self.registerReset = 0
            self.powerUpDigital = 0
            self.powerUpAnalog = 0
            self.powerUpReady = 0
            self.cycleStart = 0
            self.cycleReady = 0
            self.useExternalCrystal = 0
            self.useInternalLdo = 0

        def read(self):
            self.manager.read(self)
            return self
            
        def get_value(self):
            return ((self.registerReset & 0x1) << 0) | ((self.powerUpDigital & 0x1) << 1) | ((self.powerUpAnalog & 0x1) << 2) | ((self.powerUpReady & 0x1) << 3) | ((self.cycleStart & 0x1) << 4) | ((self.cycleReady & 0x1) << 5) | ((self.useExternalCrystal & 0x1) << 6) | ((self.useInternalLdo & 0x1) << 7)

        def set_value(self, value: int):
            self.registerReset = ((value >> 0) & 0x1)
            self.powerUpDigital = ((value >> 1) & 0x1)
            self.powerUpAnalog = ((value >> 2) & 0x1)
            self.powerUpReady = ((value >> 3) & 0x1)
            self.cycleStart = ((value >> 4) & 0x1)
            self.cycleReady = ((value >> 5) & 0x1)
            self.useExternalCrystal = ((value >> 6) & 0x1)
            self.useInternalLdo = ((value >> 7) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "RegisterReset: {} (offset: 0, width: 1)\r\n".format(self.registerReset)
            retVal += "PowerUpDigital: {} (offset: 1, width: 1)\r\n".format(self.powerUpDigital)
            retVal += "PowerUpAnalog: {} (offset: 2, width: 1)\r\n".format(self.powerUpAnalog)
            retVal += "PowerUpReady: {} (offset: 3, width: 1)\r\n".format(self.powerUpReady)
            retVal += "CycleStart: {} (offset: 4, width: 1)\r\n".format(self.cycleStart)
            retVal += "CycleReady: {} (offset: 5, width: 1)\r\n".format(self.cycleReady)
            retVal += "UseExternalCrystal: {} (offset: 6, width: 1)\r\n".format(self.useExternalCrystal)
            retVal += "UseInternalLdo: {} (offset: 7, width: 1)\r\n".format(self.useInternalLdo)
            return retVal

    class Ctrl1Register(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x01, 1, False)
            self.gain = 0
            self.vldo = 0
            self.drdySelect = 0
            self.conversionReadyPinPolarity = 0
        def get_Gains(self):
            return (Gains)Gain

        def set_Gains(self, enumVal: self.Gains):
            self.Gain = (int)enumVal

        def get_LdoVoltage(self):
            return (LdoVoltage)Vldo

        def set_LdoVoltage(self, enumVal: self.LdoVoltage):
            self.Vldo = (int)enumVal


        def read(self):
            self.manager.read(self)
            return self
            
        def get_value(self):
            return ((self.gain & 0x7) << 0) | ((self.vldo & 0x7) << 3) | ((self.drdySelect & 0x1) << 6) | ((self.conversionReadyPinPolarity & 0x1) << 7)

        def set_value(self, value: int):
            self.gain = ((value >> 0) & 0x7)
            self.vldo = ((value >> 3) & 0x7)
            self.drdySelect = ((value >> 6) & 0x1)
            self.conversionReadyPinPolarity = ((value >> 7) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "Gain: {} (offset: 0, width: 3)\r\n".format(self.gain)
            retVal += "Vldo: {} (offset: 3, width: 3)\r\n".format(self.vldo)
            retVal += "DrdySelect: {} (offset: 6, width: 1)\r\n".format(self.drdySelect)
            retVal += "ConversionReadyPinPolarity: {} (offset: 7, width: 1)\r\n".format(self.conversionReadyPinPolarity)
            return retVal

    class Ctrl2Register(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x02, 1, False)
            self.calMod = 0
            self.calStart = 0
            self.calError = 0
            self.conversionRate = 0
            self.channelSelect = 0
        def get_CalMods(self):
            return (CalMods)CalMod

        def set_CalMods(self, enumVal: self.CalMods):
            self.CalMod = (int)enumVal

        def get_ConversionRates(self):
            return (ConversionRates)ConversionRate

        def set_ConversionRates(self, enumVal: self.ConversionRates):
            self.ConversionRate = (int)enumVal


        def read(self):
            self.manager.read(self)
            return self
            
        def get_value(self):
            return ((self.calMod & 0x3) << 0) | ((self.calStart & 0x1) << 2) | ((self.calError & 0x1) << 3) | ((self.conversionRate & 0x7) << 4) | ((self.channelSelect & 0x1) << 7)

        def set_value(self, value: int):
            self.calMod = ((value >> 0) & 0x3)
            self.calStart = ((value >> 2) & 0x1)
            self.calError = ((value >> 3) & 0x1)
            self.conversionRate = ((value >> 4) & 0x7)
            self.channelSelect = ((value >> 7) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "CalMod: {} (offset: 0, width: 2)\r\n".format(self.calMod)
            retVal += "CalStart: {} (offset: 2, width: 1)\r\n".format(self.calStart)
            retVal += "CalError: {} (offset: 3, width: 1)\r\n".format(self.calError)
            retVal += "ConversionRate: {} (offset: 4, width: 3)\r\n".format(self.conversionRate)
            retVal += "ChannelSelect: {} (offset: 7, width: 1)\r\n".format(self.channelSelect)
            return retVal

    class I2cCtrlRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x11, 1, False)
            self.bgpCp = 0
            self.ts = 0
            self.boPga = 0
            self.si = 0
            self.wpd = 0
            self.spe = 0
            self.frd = 0
            self.crsd = 0

        def read(self):
            self.manager.read(self)
            return self
            
        def get_value(self):
            return ((self.bgpCp & 0x1) << 0) | ((self.ts & 0x1) << 1) | ((self.boPga & 0x1) << 2) | ((self.si & 0x1) << 3) | ((self.wpd & 0x1) << 4) | ((self.spe & 0x1) << 5) | ((self.frd & 0x1) << 6) | ((self.crsd & 0x1) << 7)

        def set_value(self, value: int):
            self.bgpCp = ((value >> 0) & 0x1)
            self.ts = ((value >> 1) & 0x1)
            self.boPga = ((value >> 2) & 0x1)
            self.si = ((value >> 3) & 0x1)
            self.wpd = ((value >> 4) & 0x1)
            self.spe = ((value >> 5) & 0x1)
            self.frd = ((value >> 6) & 0x1)
            self.crsd = ((value >> 7) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "BgpCp: {} (offset: 0, width: 1)\r\n".format(self.bgpCp)
            retVal += "Ts: {} (offset: 1, width: 1)\r\n".format(self.ts)
            retVal += "BoPga: {} (offset: 2, width: 1)\r\n".format(self.boPga)
            retVal += "Si: {} (offset: 3, width: 1)\r\n".format(self.si)
            retVal += "Wpd: {} (offset: 4, width: 1)\r\n".format(self.wpd)
            retVal += "Spe: {} (offset: 5, width: 1)\r\n".format(self.spe)
            retVal += "Frd: {} (offset: 6, width: 1)\r\n".format(self.frd)
            retVal += "Crsd: {} (offset: 7, width: 1)\r\n".format(self.crsd)
            return retVal

    class AdcResultRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x12, 3, True)
            self.value = 0

        def read(self):
            self.manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFFFFFF) << 0)

        def set_value(self, value: int):
            self.value = sign_extend((value >> 0) & 0xFFFFFF, 24)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 24)\r\n".format(self.value)
            return retVal

    class AdcRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x15, 1, False)
            self.regChp = 0
            self.adcVcm = 0
            self.regChpFreq = 0
        def get_AdcVcms(self):
            return (AdcVcms)AdcVcm

        def set_AdcVcms(self, enumVal: self.AdcVcms):
            self.AdcVcm = (int)enumVal

        def get_RegChpFreqs(self):
            return (RegChpFreqs)RegChpFreq

        def set_RegChpFreqs(self, enumVal: self.RegChpFreqs):
            self.RegChpFreq = (int)enumVal


        def read(self):
            self.manager.read(self)
            return self
            
        def get_value(self):
            return ((self.regChp & 0x3) << 0) | ((self.adcVcm & 0x3) << 2) | ((self.regChpFreq & 0x3) << 4)

        def set_value(self, value: int):
            self.regChp = ((value >> 0) & 0x3)
            self.adcVcm = ((value >> 2) & 0x3)
            self.regChpFreq = ((value >> 4) & 0x3)

        def __str__(self):
            retVal = ""
            retVal += "RegChp: {} (offset: 0, width: 2)\r\n".format(self.regChp)
            retVal += "AdcVcm: {} (offset: 2, width: 2)\r\n".format(self.adcVcm)
            retVal += "RegChpFreq: {} (offset: 4, width: 2)\r\n".format(self.regChpFreq)
            return retVal

    class PgaRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x1B, 1, False)
            self.disableChopper = 0
            self.pgaInv = 0
            self.pgaBypass = 0
            self.ldoMode = 0
            self.rdOptSel = 0

        def read(self):
            self.manager.read(self)
            return self
            
        def get_value(self):
            return ((self.disableChopper & 0x1) << 0) | ((self.pgaInv & 0x1) << 3) | ((self.pgaBypass & 0x1) << 4) | ((self.ldoMode & 0x1) << 5) | ((self.rdOptSel & 0x1) << 6)

        def set_value(self, value: int):
            self.disableChopper = ((value >> 0) & 0x1)
            self.pgaInv = ((value >> 3) & 0x1)
            self.pgaBypass = ((value >> 4) & 0x1)
            self.ldoMode = ((value >> 5) & 0x1)
            self.rdOptSel = ((value >> 6) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "DisableChopper: {} (offset: 0, width: 1)\r\n".format(self.disableChopper)
            retVal += "PgaInv: {} (offset: 3, width: 1)\r\n".format(self.pgaInv)
            retVal += "PgaBypass: {} (offset: 4, width: 1)\r\n".format(self.pgaBypass)
            retVal += "LdoMode: {} (offset: 5, width: 1)\r\n".format(self.ldoMode)
            retVal += "RdOptSel: {} (offset: 6, width: 1)\r\n".format(self.rdOptSel)
            return retVal

    class PowerCtrlRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x1C, 1, False)
            self.pgaCurr = 0
            self.adcCurr = 0
            self.masterBiasCurr = 0
            self.pgaCapEn = 0

        def read(self):
            self.manager.read(self)
            return self
            
        def get_value(self):
            return ((self.pgaCurr & 0x3) << 0) | ((self.adcCurr & 0x3) << 2) | ((self.masterBiasCurr & 0x7) << 4) | ((self.pgaCapEn & 0x1) << 7)

        def set_value(self, value: int):
            self.pgaCurr = ((value >> 0) & 0x3)
            self.adcCurr = ((value >> 2) & 0x3)
            self.masterBiasCurr = ((value >> 4) & 0x7)
            self.pgaCapEn = ((value >> 7) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "PgaCurr: {} (offset: 0, width: 2)\r\n".format(self.pgaCurr)
            retVal += "AdcCurr: {} (offset: 2, width: 2)\r\n".format(self.adcCurr)
            retVal += "MasterBiasCurr: {} (offset: 4, width: 3)\r\n".format(self.masterBiasCurr)
            retVal += "PgaCapEn: {} (offset: 7, width: 1)\r\n".format(self.pgaCapEn)
            return retVal

