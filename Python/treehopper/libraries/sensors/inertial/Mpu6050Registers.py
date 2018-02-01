from treehopper.api import *
from treehopper.utils import *
from treehopper.libraries import RegisterManager, Register, SMBusDevice
from treehopper.libraries.Register import sign_extend

class ExtSyncSets:
    Disabled = 0
    TempOutL = 1
    GyroXoutL = 2
    GyroYoutL = 3
    GyroZoutL = 4
    AccelXoutL = 5
    AccelYoutL = 6
    AccelZoutL = 7
    
class GyroFsSels:
    Dps_250 = 0
    Dps_500 = 1
    Dps_1000 = 2
    Dps_2000 = 3
    
class AccelFsSels:
    Fs_2g = 0
    Fs_4g = 1
    Fs_8g = 2
    Fs_16g = 3
    
class ClockSels:
    InternalOscillator = 0
    AutoSelect = 1
    Reset = 7
    
class Mpu6050Registers(RegisterManager):
    def __init__(self, dev: SMBusDevice):
        RegisterManager.__init__(self, dev, True)
        self.self_test_x_gyro = self.Self_test_x_gyroRegister(self)
        self.registers.append(self.self_test_x_gyro)
        self.self_test_y_gyro = self.Self_test_y_gyroRegister(self)
        self.registers.append(self.self_test_y_gyro)
        self.self_test_z_gyro = self.Self_test_z_gyroRegister(self)
        self.registers.append(self.self_test_z_gyro)
        self.self_test_x_accel = self.Self_test_x_accelRegister(self)
        self.registers.append(self.self_test_x_accel)
        self.self_test_y_accel = self.Self_test_y_accelRegister(self)
        self.registers.append(self.self_test_y_accel)
        self.self_test_z_accel = self.Self_test_z_accelRegister(self)
        self.registers.append(self.self_test_z_accel)
        self.xGyroOffsUsr = self.XGyroOffsUsrRegister(self)
        self.registers.append(self.xGyroOffsUsr)
        self.yGyroOffsUsr = self.YGyroOffsUsrRegister(self)
        self.registers.append(self.yGyroOffsUsr)
        self.zGyroOffsUsr = self.ZGyroOffsUsrRegister(self)
        self.registers.append(self.zGyroOffsUsr)
        self.sampleRateDivider = self.SampleRateDividerRegister(self)
        self.registers.append(self.sampleRateDivider)
        self.configuration = self.ConfigurationRegister(self)
        self.registers.append(self.configuration)
        self.gyroConfig = self.GyroConfigRegister(self)
        self.registers.append(self.gyroConfig)
        self.accelConfig = self.AccelConfigRegister(self)
        self.registers.append(self.accelConfig)
        self.accelConfig2 = self.AccelConfig2Register(self)
        self.registers.append(self.accelConfig2)
        self.lowPowerAccelerometerOdrControl = self.LowPowerAccelerometerOdrControlRegister(self)
        self.registers.append(self.lowPowerAccelerometerOdrControl)
        self.womThreshold = self.WomThresholdRegister(self)
        self.registers.append(self.womThreshold)
        self.fifoEnable = self.FifoEnableRegister(self)
        self.registers.append(self.fifoEnable)
        self.i2cMasterControl = self.I2cMasterControlRegister(self)
        self.registers.append(self.i2cMasterControl)
        self.i2cSlv0Addr = self.I2cSlv0AddrRegister(self)
        self.registers.append(self.i2cSlv0Addr)
        self.i2cSlv0Reg = self.I2cSlv0RegRegister(self)
        self.registers.append(self.i2cSlv0Reg)
        self.i2cSlv0Ctrl = self.I2cSlv0CtrlRegister(self)
        self.registers.append(self.i2cSlv0Ctrl)
        self.i2cSlv1Addr = self.I2cSlv1AddrRegister(self)
        self.registers.append(self.i2cSlv1Addr)
        self.i2cSlv1Reg = self.I2cSlv1RegRegister(self)
        self.registers.append(self.i2cSlv1Reg)
        self.i2cSlv1Ctrl = self.I2cSlv1CtrlRegister(self)
        self.registers.append(self.i2cSlv1Ctrl)
        self.i2cSlv2Addr = self.I2cSlv2AddrRegister(self)
        self.registers.append(self.i2cSlv2Addr)
        self.i2cSlv2Reg = self.I2cSlv2RegRegister(self)
        self.registers.append(self.i2cSlv2Reg)
        self.i2cSlv2Ctrl = self.I2cSlv2CtrlRegister(self)
        self.registers.append(self.i2cSlv2Ctrl)
        self.i2cSlv3Addr = self.I2cSlv3AddrRegister(self)
        self.registers.append(self.i2cSlv3Addr)
        self.i2cSlv3Reg = self.I2cSlv3RegRegister(self)
        self.registers.append(self.i2cSlv3Reg)
        self.i2cSlv3Ctrl = self.I2cSlv3CtrlRegister(self)
        self.registers.append(self.i2cSlv3Ctrl)
        self.i2cSlv4Addr = self.I2cSlv4AddrRegister(self)
        self.registers.append(self.i2cSlv4Addr)
        self.i2cSlv4Reg = self.I2cSlv4RegRegister(self)
        self.registers.append(self.i2cSlv4Reg)
        self.i2cSlv4Do = self.I2cSlv4DoRegister(self)
        self.registers.append(self.i2cSlv4Do)
        self.i2cSlv4Ctrl = self.I2cSlv4CtrlRegister(self)
        self.registers.append(self.i2cSlv4Ctrl)
        self.i2cSlv4Di = self.I2cSlv4DiRegister(self)
        self.registers.append(self.i2cSlv4Di)
        self.i2cMstStatus = self.I2cMstStatusRegister(self)
        self.registers.append(self.i2cMstStatus)
        self.intPinCfg = self.IntPinCfgRegister(self)
        self.registers.append(self.intPinCfg)
        self.intEnable = self.IntEnableRegister(self)
        self.registers.append(self.intEnable)
        self.intStatus = self.IntStatusRegister(self)
        self.registers.append(self.intStatus)
        self.accel_x = self.Accel_xRegister(self)
        self.registers.append(self.accel_x)
        self.accel_y = self.Accel_yRegister(self)
        self.registers.append(self.accel_y)
        self.accel_z = self.Accel_zRegister(self)
        self.registers.append(self.accel_z)
        self.temp = self.TempRegister(self)
        self.registers.append(self.temp)
        self.gyro_x = self.Gyro_xRegister(self)
        self.registers.append(self.gyro_x)
        self.gyro_y = self.Gyro_yRegister(self)
        self.registers.append(self.gyro_y)
        self.gyro_z = self.Gyro_zRegister(self)
        self.registers.append(self.gyro_z)
        self.extSensData = self.ExtSensDataRegister(self)
        self.registers.append(self.extSensData)
        self.i2cSlv0do = self.I2cSlv0doRegister(self)
        self.registers.append(self.i2cSlv0do)
        self.i2cSlv1do = self.I2cSlv1doRegister(self)
        self.registers.append(self.i2cSlv1do)
        self.i2cSlv2do = self.I2cSlv2doRegister(self)
        self.registers.append(self.i2cSlv2do)
        self.i2cSlv3do = self.I2cSlv3doRegister(self)
        self.registers.append(self.i2cSlv3do)
        self.i2cMstDelayCtrl = self.I2cMstDelayCtrlRegister(self)
        self.registers.append(self.i2cMstDelayCtrl)
        self.signalPathReset = self.SignalPathResetRegister(self)
        self.registers.append(self.signalPathReset)
        self.accelIntCtrl = self.AccelIntCtrlRegister(self)
        self.registers.append(self.accelIntCtrl)
        self.userCtrl = self.UserCtrlRegister(self)
        self.registers.append(self.userCtrl)
        self.powerMgmt1 = self.PowerMgmt1Register(self)
        self.registers.append(self.powerMgmt1)
        self.powerMgmt2 = self.PowerMgmt2Register(self)
        self.registers.append(self.powerMgmt2)
        self.fifoCount = self.FifoCountRegister(self)
        self.registers.append(self.fifoCount)
        self.fifoRW = self.FifoRWRegister(self)
        self.registers.append(self.fifoRW)
        self.whoAmI = self.WhoAmIRegister(self)
        self.registers.append(self.whoAmI)
        self.xAccelOffset = self.XAccelOffsetRegister(self)
        self.registers.append(self.xAccelOffset)
        self.yAccelOffset = self.YAccelOffsetRegister(self)
        self.registers.append(self.yAccelOffset)
        self.zAccelOffset = self.ZAccelOffsetRegister(self)
        self.registers.append(self.zAccelOffset)

    class Self_test_x_gyroRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x00, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 8)\r\n".format(self.value)
            return retVal

    class Self_test_y_gyroRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x01, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 8)\r\n".format(self.value)
            return retVal

    class Self_test_z_gyroRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x02, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 8)\r\n".format(self.value)
            return retVal

    class Self_test_x_accelRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x0d, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 8)\r\n".format(self.value)
            return retVal

    class Self_test_y_accelRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x0e, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 8)\r\n".format(self.value)
            return retVal

    class Self_test_z_accelRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x0f, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 8)\r\n".format(self.value)
            return retVal

    class XGyroOffsUsrRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x13, 2, False)
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

    class YGyroOffsUsrRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x15, 2, False)
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

    class ZGyroOffsUsrRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x17, 2, False)
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

    class SampleRateDividerRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x19, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 8)\r\n".format(self.value)
            return retVal

    class ConfigurationRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x1a, 1, False)
            self.dlpf = 0
            self.extSyncSet = 0
            self.fifoMode = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.dlpf & 0x7) << 0) | ((self.extSyncSet & 0x7) << 3) | ((self.fifoMode & 0x1) << 6)

        def set_value(self, value: int):
            self.dlpf = ((value >> 0) & 0x7)
            self.extSyncSet = ((value >> 3) & 0x7)
            self.fifoMode = ((value >> 6) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "Dlpf: {} (offset: 0, width: 3)\r\n".format(self.dlpf)
            retVal += "ExtSyncSet: {} (offset: 3, width: 3)\r\n".format(self.extSyncSet)
            retVal += "FifoMode: {} (offset: 6, width: 1)\r\n".format(self.fifoMode)
            return retVal

    class GyroConfigRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x1b, 1, False)
            self.fChoiceBypass = 0
            self.gyroFsSel = 0
            self.zGyroCten = 0
            self.yGyroCten = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.fChoiceBypass & 0x3) << 0) | ((self.gyroFsSel & 0x3) << 3) | ((self.zGyroCten & 0x1) << 5) | ((self.yGyroCten & 0x1) << 6)

        def set_value(self, value: int):
            self.fChoiceBypass = ((value >> 0) & 0x3)
            self.gyroFsSel = ((value >> 3) & 0x3)
            self.zGyroCten = ((value >> 5) & 0x1)
            self.yGyroCten = ((value >> 6) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "FChoiceBypass: {} (offset: 0, width: 2)\r\n".format(self.fChoiceBypass)
            retVal += "GyroFsSel: {} (offset: 3, width: 2)\r\n".format(self.gyroFsSel)
            retVal += "ZGyroCten: {} (offset: 5, width: 1)\r\n".format(self.zGyroCten)
            retVal += "YGyroCten: {} (offset: 6, width: 1)\r\n".format(self.yGyroCten)
            return retVal

    class AccelConfigRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x1c, 1, False)
            self.accelFsSel = 0
            self.accelZselfTest = 0
            self.accelYselfTest = 0
            self.accelXselfTest = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.accelFsSel & 0x3) << 3) | ((self.accelZselfTest & 0x1) << 5) | ((self.accelYselfTest & 0x1) << 6) | ((self.accelXselfTest & 0x1) << 7)

        def set_value(self, value: int):
            self.accelFsSel = ((value >> 3) & 0x3)
            self.accelZselfTest = ((value >> 5) & 0x1)
            self.accelYselfTest = ((value >> 6) & 0x1)
            self.accelXselfTest = ((value >> 7) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "AccelFsSel: {} (offset: 3, width: 2)\r\n".format(self.accelFsSel)
            retVal += "AccelZselfTest: {} (offset: 5, width: 1)\r\n".format(self.accelZselfTest)
            retVal += "AccelYselfTest: {} (offset: 6, width: 1)\r\n".format(self.accelYselfTest)
            retVal += "AccelXselfTest: {} (offset: 7, width: 1)\r\n".format(self.accelXselfTest)
            return retVal

    class AccelConfig2Register(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x1d, 1, False)
            self.dlpfCfg = 0
            self.accelFchoice = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.dlpfCfg & 0x7) << 0) | ((self.accelFchoice & 0x1) << 3)

        def set_value(self, value: int):
            self.dlpfCfg = ((value >> 0) & 0x7)
            self.accelFchoice = ((value >> 3) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "DlpfCfg: {} (offset: 0, width: 3)\r\n".format(self.dlpfCfg)
            retVal += "AccelFchoice: {} (offset: 3, width: 1)\r\n".format(self.accelFchoice)
            return retVal

    class LowPowerAccelerometerOdrControlRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x1e, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 4)\r\n".format(self.value)
            return retVal

    class WomThresholdRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x1f, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 8)\r\n".format(self.value)
            return retVal

    class FifoEnableRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x23, 1, False)
            self.slv0 = 0
            self.slv1 = 0
            self.slv2 = 0
            self.accel = 0
            self.gyroZout = 0
            self.gyroYout = 0
            self.gyroXout = 0
            self.tempOut = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.slv0 & 0x1) << 0) | ((self.slv1 & 0x1) << 1) | ((self.slv2 & 0x1) << 2) | ((self.accel & 0x1) << 3) | ((self.gyroZout & 0x1) << 4) | ((self.gyroYout & 0x1) << 5) | ((self.gyroXout & 0x1) << 6) | ((self.tempOut & 0x1) << 7)

        def set_value(self, value: int):
            self.slv0 = ((value >> 0) & 0x1)
            self.slv1 = ((value >> 1) & 0x1)
            self.slv2 = ((value >> 2) & 0x1)
            self.accel = ((value >> 3) & 0x1)
            self.gyroZout = ((value >> 4) & 0x1)
            self.gyroYout = ((value >> 5) & 0x1)
            self.gyroXout = ((value >> 6) & 0x1)
            self.tempOut = ((value >> 7) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "Slv0: {} (offset: 0, width: 1)\r\n".format(self.slv0)
            retVal += "Slv1: {} (offset: 1, width: 1)\r\n".format(self.slv1)
            retVal += "Slv2: {} (offset: 2, width: 1)\r\n".format(self.slv2)
            retVal += "Accel: {} (offset: 3, width: 1)\r\n".format(self.accel)
            retVal += "GyroZout: {} (offset: 4, width: 1)\r\n".format(self.gyroZout)
            retVal += "GyroYout: {} (offset: 5, width: 1)\r\n".format(self.gyroYout)
            retVal += "GyroXout: {} (offset: 6, width: 1)\r\n".format(self.gyroXout)
            retVal += "TempOut: {} (offset: 7, width: 1)\r\n".format(self.tempOut)
            return retVal

    class I2cMasterControlRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x24, 1, False)
            self.i2cMasterClock = 0
            self.i2cMstPnsr = 0
            self.slv3FifoEn = 0
            self.waitForEs = 0
            self.multMstEn = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.i2cMasterClock & 0xF) << 0) | ((self.i2cMstPnsr & 0x1) << 4) | ((self.slv3FifoEn & 0x1) << 5) | ((self.waitForEs & 0x1) << 6) | ((self.multMstEn & 0x1) << 7)

        def set_value(self, value: int):
            self.i2cMasterClock = ((value >> 0) & 0xF)
            self.i2cMstPnsr = ((value >> 4) & 0x1)
            self.slv3FifoEn = ((value >> 5) & 0x1)
            self.waitForEs = ((value >> 6) & 0x1)
            self.multMstEn = ((value >> 7) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "I2cMasterClock: {} (offset: 0, width: 4)\r\n".format(self.i2cMasterClock)
            retVal += "I2cMstPnsr: {} (offset: 4, width: 1)\r\n".format(self.i2cMstPnsr)
            retVal += "Slv3FifoEn: {} (offset: 5, width: 1)\r\n".format(self.slv3FifoEn)
            retVal += "WaitForEs: {} (offset: 6, width: 1)\r\n".format(self.waitForEs)
            retVal += "MultMstEn: {} (offset: 7, width: 1)\r\n".format(self.multMstEn)
            return retVal

    class I2cSlv0AddrRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x25, 1, False)
            self.id = 0
            self.rnw = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.id & 0x7F) << 0) | ((self.rnw & 0x1) << 7)

        def set_value(self, value: int):
            self.id = ((value >> 0) & 0x7F)
            self.rnw = ((value >> 7) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "Id: {} (offset: 0, width: 7)\r\n".format(self.id)
            retVal += "Rnw: {} (offset: 7, width: 1)\r\n".format(self.rnw)
            return retVal

    class I2cSlv0RegRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x26, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 8)\r\n".format(self.value)
            return retVal

    class I2cSlv0CtrlRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x27, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 8)\r\n".format(self.value)
            return retVal

    class I2cSlv1AddrRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x28, 1, False)
            self.id = 0
            self.rnw = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.id & 0x7F) << 0) | ((self.rnw & 0x1) << 7)

        def set_value(self, value: int):
            self.id = ((value >> 0) & 0x7F)
            self.rnw = ((value >> 7) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "Id: {} (offset: 0, width: 7)\r\n".format(self.id)
            retVal += "Rnw: {} (offset: 7, width: 1)\r\n".format(self.rnw)
            return retVal

    class I2cSlv1RegRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x29, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 8)\r\n".format(self.value)
            return retVal

    class I2cSlv1CtrlRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x2a, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 8)\r\n".format(self.value)
            return retVal

    class I2cSlv2AddrRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x2b, 1, False)
            self.id = 0
            self.rnw = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.id & 0x7F) << 0) | ((self.rnw & 0x1) << 7)

        def set_value(self, value: int):
            self.id = ((value >> 0) & 0x7F)
            self.rnw = ((value >> 7) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "Id: {} (offset: 0, width: 7)\r\n".format(self.id)
            retVal += "Rnw: {} (offset: 7, width: 1)\r\n".format(self.rnw)
            return retVal

    class I2cSlv2RegRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x2c, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 8)\r\n".format(self.value)
            return retVal

    class I2cSlv2CtrlRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x2d, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 8)\r\n".format(self.value)
            return retVal

    class I2cSlv3AddrRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x2e, 1, False)
            self.id = 0
            self.rnw = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.id & 0x7F) << 0) | ((self.rnw & 0x1) << 7)

        def set_value(self, value: int):
            self.id = ((value >> 0) & 0x7F)
            self.rnw = ((value >> 7) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "Id: {} (offset: 0, width: 7)\r\n".format(self.id)
            retVal += "Rnw: {} (offset: 7, width: 1)\r\n".format(self.rnw)
            return retVal

    class I2cSlv3RegRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x2f, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 8)\r\n".format(self.value)
            return retVal

    class I2cSlv3CtrlRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x30, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 8)\r\n".format(self.value)
            return retVal

    class I2cSlv4AddrRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x31, 1, False)
            self.id = 0
            self.rnw = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.id & 0x7F) << 0) | ((self.rnw & 0x1) << 7)

        def set_value(self, value: int):
            self.id = ((value >> 0) & 0x7F)
            self.rnw = ((value >> 7) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "Id: {} (offset: 0, width: 7)\r\n".format(self.id)
            retVal += "Rnw: {} (offset: 7, width: 1)\r\n".format(self.rnw)
            return retVal

    class I2cSlv4RegRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x32, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 8)\r\n".format(self.value)
            return retVal

    class I2cSlv4DoRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x33, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 8)\r\n".format(self.value)
            return retVal

    class I2cSlv4CtrlRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x34, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 8)\r\n".format(self.value)
            return retVal

    class I2cSlv4DiRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x35, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 8)\r\n".format(self.value)
            return retVal

    class I2cMstStatusRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x36, 1, False)
            self.slv0Nack = 0
            self.slv1Nack = 0
            self.slv2Nack = 0
            self.slv3Nack = 0
            self.slv4Nack = 0
            self.lostArb = 0
            self.slv4Done = 0
            self.passThrough = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.slv0Nack & 0x1) << 0) | ((self.slv1Nack & 0x1) << 1) | ((self.slv2Nack & 0x1) << 2) | ((self.slv3Nack & 0x1) << 3) | ((self.slv4Nack & 0x1) << 4) | ((self.lostArb & 0x1) << 5) | ((self.slv4Done & 0x1) << 6) | ((self.passThrough & 0x1) << 7)

        def set_value(self, value: int):
            self.slv0Nack = ((value >> 0) & 0x1)
            self.slv1Nack = ((value >> 1) & 0x1)
            self.slv2Nack = ((value >> 2) & 0x1)
            self.slv3Nack = ((value >> 3) & 0x1)
            self.slv4Nack = ((value >> 4) & 0x1)
            self.lostArb = ((value >> 5) & 0x1)
            self.slv4Done = ((value >> 6) & 0x1)
            self.passThrough = ((value >> 7) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "Slv0Nack: {} (offset: 0, width: 1)\r\n".format(self.slv0Nack)
            retVal += "Slv1Nack: {} (offset: 1, width: 1)\r\n".format(self.slv1Nack)
            retVal += "Slv2Nack: {} (offset: 2, width: 1)\r\n".format(self.slv2Nack)
            retVal += "Slv3Nack: {} (offset: 3, width: 1)\r\n".format(self.slv3Nack)
            retVal += "Slv4Nack: {} (offset: 4, width: 1)\r\n".format(self.slv4Nack)
            retVal += "LostArb: {} (offset: 5, width: 1)\r\n".format(self.lostArb)
            retVal += "Slv4Done: {} (offset: 6, width: 1)\r\n".format(self.slv4Done)
            retVal += "PassThrough: {} (offset: 7, width: 1)\r\n".format(self.passThrough)
            return retVal

    class IntPinCfgRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x37, 1, False)
            self.bypassEn = 0
            self.fsyncIntModeEnable = 0
            self.actlFsync = 0
            self.intAnyRd2Clear = 0
            self.latchIntEn = 0
            self.open = 0
            self.actl = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.bypassEn & 0x1) << 1) | ((self.fsyncIntModeEnable & 0x1) << 2) | ((self.actlFsync & 0x1) << 3) | ((self.intAnyRd2Clear & 0x1) << 4) | ((self.latchIntEn & 0x1) << 5) | ((self.open & 0x1) << 6) | ((self.actl & 0x1) << 7)

        def set_value(self, value: int):
            self.bypassEn = ((value >> 1) & 0x1)
            self.fsyncIntModeEnable = ((value >> 2) & 0x1)
            self.actlFsync = ((value >> 3) & 0x1)
            self.intAnyRd2Clear = ((value >> 4) & 0x1)
            self.latchIntEn = ((value >> 5) & 0x1)
            self.open = ((value >> 6) & 0x1)
            self.actl = ((value >> 7) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "BypassEn: {} (offset: 1, width: 1)\r\n".format(self.bypassEn)
            retVal += "FsyncIntModeEnable: {} (offset: 2, width: 1)\r\n".format(self.fsyncIntModeEnable)
            retVal += "ActlFsync: {} (offset: 3, width: 1)\r\n".format(self.actlFsync)
            retVal += "IntAnyRd2Clear: {} (offset: 4, width: 1)\r\n".format(self.intAnyRd2Clear)
            retVal += "LatchIntEn: {} (offset: 5, width: 1)\r\n".format(self.latchIntEn)
            retVal += "Open: {} (offset: 6, width: 1)\r\n".format(self.open)
            retVal += "Actl: {} (offset: 7, width: 1)\r\n".format(self.actl)
            return retVal

    class IntEnableRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x38, 1, False)
            self.RawReadyEnable = 0
            self.fsyncIntEnable = 0
            self.fifoIntEnable = 0
            self.fifoOverflowEnable = 0
            self.womEnable = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.RawReadyEnable & 0x1) << 0) | ((self.fsyncIntEnable & 0x1) << 2) | ((self.fifoIntEnable & 0x1) << 3) | ((self.fifoOverflowEnable & 0x1) << 4) | ((self.womEnable & 0x1) << 1)

        def set_value(self, value: int):
            self.RawReadyEnable = ((value >> 0) & 0x1)
            self.fsyncIntEnable = ((value >> 2) & 0x1)
            self.fifoIntEnable = ((value >> 3) & 0x1)
            self.fifoOverflowEnable = ((value >> 4) & 0x1)
            self.womEnable = ((value >> 1) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "RawReadyEnable: {} (offset: 0, width: 1)\r\n".format(self.RawReadyEnable)
            retVal += "FsyncIntEnable: {} (offset: 2, width: 1)\r\n".format(self.fsyncIntEnable)
            retVal += "FifoIntEnable: {} (offset: 3, width: 1)\r\n".format(self.fifoIntEnable)
            retVal += "FifoOverflowEnable: {} (offset: 4, width: 1)\r\n".format(self.fifoOverflowEnable)
            retVal += "WomEnable: {} (offset: 1, width: 1)\r\n".format(self.womEnable)
            return retVal

    class IntStatusRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x3a, 1, False)
            self.rawDataReadyInt = 0
            self.fsyncInt = 0
            self.fifoOverflowInt = 0
            self.womInt = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.rawDataReadyInt & 0x1) << 0) | ((self.fsyncInt & 0x1) << 2) | ((self.fifoOverflowInt & 0x1) << 3) | ((self.womInt & 0x1) << 1)

        def set_value(self, value: int):
            self.rawDataReadyInt = ((value >> 0) & 0x1)
            self.fsyncInt = ((value >> 2) & 0x1)
            self.fifoOverflowInt = ((value >> 3) & 0x1)
            self.womInt = ((value >> 1) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "RawDataReadyInt: {} (offset: 0, width: 1)\r\n".format(self.rawDataReadyInt)
            retVal += "FsyncInt: {} (offset: 2, width: 1)\r\n".format(self.fsyncInt)
            retVal += "FifoOverflowInt: {} (offset: 3, width: 1)\r\n".format(self.fifoOverflowInt)
            retVal += "WomInt: {} (offset: 1, width: 1)\r\n".format(self.womInt)
            return retVal

    class Accel_xRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x3b, 2, True)
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

    class Accel_yRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x3d, 2, True)
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

    class Accel_zRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x3f, 2, True)
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

    class TempRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x41, 2, True)
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

    class Gyro_xRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x43, 2, True)
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

    class Gyro_yRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x45, 2, True)
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

    class Gyro_zRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x47, 2, True)
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

    class ExtSensDataRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x49, 24, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0x0) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0x0)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 192)\r\n".format(self.value)
            return retVal

    class I2cSlv0doRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x63, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 8)\r\n".format(self.value)
            return retVal

    class I2cSlv1doRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x64, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 8)\r\n".format(self.value)
            return retVal

    class I2cSlv2doRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x65, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 8)\r\n".format(self.value)
            return retVal

    class I2cSlv3doRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x66, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 8)\r\n".format(self.value)
            return retVal

    class I2cMstDelayCtrlRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x67, 1, False)
            self.slv0DelayEn = 0
            self.slv1DelayEn = 0
            self.slv2DelayEn = 0
            self.slv3DelayEn = 0
            self.slv4DelayEn = 0
            self.delayEsShadow = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.slv0DelayEn & 0x1) << 0) | ((self.slv1DelayEn & 0x1) << 1) | ((self.slv2DelayEn & 0x1) << 2) | ((self.slv3DelayEn & 0x1) << 3) | ((self.slv4DelayEn & 0x1) << 4) | ((self.delayEsShadow & 0x1) << 2)

        def set_value(self, value: int):
            self.slv0DelayEn = ((value >> 0) & 0x1)
            self.slv1DelayEn = ((value >> 1) & 0x1)
            self.slv2DelayEn = ((value >> 2) & 0x1)
            self.slv3DelayEn = ((value >> 3) & 0x1)
            self.slv4DelayEn = ((value >> 4) & 0x1)
            self.delayEsShadow = ((value >> 2) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "Slv0DelayEn: {} (offset: 0, width: 1)\r\n".format(self.slv0DelayEn)
            retVal += "Slv1DelayEn: {} (offset: 1, width: 1)\r\n".format(self.slv1DelayEn)
            retVal += "Slv2DelayEn: {} (offset: 2, width: 1)\r\n".format(self.slv2DelayEn)
            retVal += "Slv3DelayEn: {} (offset: 3, width: 1)\r\n".format(self.slv3DelayEn)
            retVal += "Slv4DelayEn: {} (offset: 4, width: 1)\r\n".format(self.slv4DelayEn)
            retVal += "DelayEsShadow: {} (offset: 2, width: 1)\r\n".format(self.delayEsShadow)
            return retVal

    class SignalPathResetRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x68, 1, False)
            self.tempReset = 0
            self.accelReset = 0
            self.gyroReset = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.tempReset & 0x1) << 0) | ((self.accelReset & 0x1) << 1) | ((self.gyroReset & 0x1) << 2)

        def set_value(self, value: int):
            self.tempReset = ((value >> 0) & 0x1)
            self.accelReset = ((value >> 1) & 0x1)
            self.gyroReset = ((value >> 2) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "TempReset: {} (offset: 0, width: 1)\r\n".format(self.tempReset)
            retVal += "AccelReset: {} (offset: 1, width: 1)\r\n".format(self.accelReset)
            retVal += "GyroReset: {} (offset: 2, width: 1)\r\n".format(self.gyroReset)
            return retVal

    class AccelIntCtrlRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x69, 1, False)
            self.accelIntelMode = 0
            self.accelIntelEnable = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.accelIntelMode & 0x1) << 6) | ((self.accelIntelEnable & 0x1) << 7)

        def set_value(self, value: int):
            self.accelIntelMode = ((value >> 6) & 0x1)
            self.accelIntelEnable = ((value >> 7) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "AccelIntelMode: {} (offset: 6, width: 1)\r\n".format(self.accelIntelMode)
            retVal += "AccelIntelEnable: {} (offset: 7, width: 1)\r\n".format(self.accelIntelEnable)
            return retVal

    class UserCtrlRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x6a, 1, False)
            self.sigConditionReset = 0
            self.i2cMasterReset = 0
            self.fifoReset = 0
            self.i2cIfDisable = 0
            self.i2cMasterEnable = 0
            self.fifoEnable = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.sigConditionReset & 0x1) << 0) | ((self.i2cMasterReset & 0x1) << 1) | ((self.fifoReset & 0x1) << 2) | ((self.i2cIfDisable & 0x1) << 1) | ((self.i2cMasterEnable & 0x1) << 2) | ((self.fifoEnable & 0x1) << 3)

        def set_value(self, value: int):
            self.sigConditionReset = ((value >> 0) & 0x1)
            self.i2cMasterReset = ((value >> 1) & 0x1)
            self.fifoReset = ((value >> 2) & 0x1)
            self.i2cIfDisable = ((value >> 1) & 0x1)
            self.i2cMasterEnable = ((value >> 2) & 0x1)
            self.fifoEnable = ((value >> 3) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "SigConditionReset: {} (offset: 0, width: 1)\r\n".format(self.sigConditionReset)
            retVal += "I2cMasterReset: {} (offset: 1, width: 1)\r\n".format(self.i2cMasterReset)
            retVal += "FifoReset: {} (offset: 2, width: 1)\r\n".format(self.fifoReset)
            retVal += "I2cIfDisable: {} (offset: 1, width: 1)\r\n".format(self.i2cIfDisable)
            retVal += "I2cMasterEnable: {} (offset: 2, width: 1)\r\n".format(self.i2cMasterEnable)
            retVal += "FifoEnable: {} (offset: 3, width: 1)\r\n".format(self.fifoEnable)
            return retVal

    class PowerMgmt1Register(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x6b, 1, False)
            self.clockSel = 0
            self.powerDownPtat = 0
            self.gyroStandby = 0
            self.cycle = 0
            self.sleep = 0
            self.reset = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.clockSel & 0x7) << 0) | ((self.powerDownPtat & 0x1) << 3) | ((self.gyroStandby & 0x1) << 4) | ((self.cycle & 0x1) << 5) | ((self.sleep & 0x1) << 6) | ((self.reset & 0x1) << 7)

        def set_value(self, value: int):
            self.clockSel = ((value >> 0) & 0x7)
            self.powerDownPtat = ((value >> 3) & 0x1)
            self.gyroStandby = ((value >> 4) & 0x1)
            self.cycle = ((value >> 5) & 0x1)
            self.sleep = ((value >> 6) & 0x1)
            self.reset = ((value >> 7) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "ClockSel: {} (offset: 0, width: 3)\r\n".format(self.clockSel)
            retVal += "PowerDownPtat: {} (offset: 3, width: 1)\r\n".format(self.powerDownPtat)
            retVal += "GyroStandby: {} (offset: 4, width: 1)\r\n".format(self.gyroStandby)
            retVal += "Cycle: {} (offset: 5, width: 1)\r\n".format(self.cycle)
            retVal += "Sleep: {} (offset: 6, width: 1)\r\n".format(self.sleep)
            retVal += "Reset: {} (offset: 7, width: 1)\r\n".format(self.reset)
            return retVal

    class PowerMgmt2Register(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x6c, 1, False)
            self.disableZG = 0
            self.disableYG = 0
            self.disableXG = 0
            self.disableZA = 0
            self.disableYA = 0
            self.disableXA = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.disableZG & 0x1) << 0) | ((self.disableYG & 0x1) << 1) | ((self.disableXG & 0x1) << 2) | ((self.disableZA & 0x1) << 3) | ((self.disableYA & 0x1) << 4) | ((self.disableXA & 0x1) << 5)

        def set_value(self, value: int):
            self.disableZG = ((value >> 0) & 0x1)
            self.disableYG = ((value >> 1) & 0x1)
            self.disableXG = ((value >> 2) & 0x1)
            self.disableZA = ((value >> 3) & 0x1)
            self.disableYA = ((value >> 4) & 0x1)
            self.disableXA = ((value >> 5) & 0x1)

        def __str__(self):
            retVal = ""
            retVal += "DisableZG: {} (offset: 0, width: 1)\r\n".format(self.disableZG)
            retVal += "DisableYG: {} (offset: 1, width: 1)\r\n".format(self.disableYG)
            retVal += "DisableXG: {} (offset: 2, width: 1)\r\n".format(self.disableXG)
            retVal += "DisableZA: {} (offset: 3, width: 1)\r\n".format(self.disableZA)
            retVal += "DisableYA: {} (offset: 4, width: 1)\r\n".format(self.disableYA)
            retVal += "DisableXA: {} (offset: 5, width: 1)\r\n".format(self.disableXA)
            return retVal

    class FifoCountRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x72, 2, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0x1FFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0x1FFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 13)\r\n".format(self.value)
            return retVal

    class FifoRWRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x74, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 8)\r\n".format(self.value)
            return retVal

    class WhoAmIRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x75, 1, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0xFF) << 0)

        def set_value(self, value: int):
            self.value = ((value >> 0) & 0xFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 0, width: 8)\r\n".format(self.value)
            return retVal

    class XAccelOffsetRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x77, 2, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0x7FFF) << 1)

        def set_value(self, value: int):
            self.value = ((value >> 1) & 0x7FFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 1, width: 15)\r\n".format(self.value)
            return retVal

    class YAccelOffsetRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x7a, 2, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0x7FFF) << 1)

        def set_value(self, value: int):
            self.value = ((value >> 1) & 0x7FFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 1, width: 15)\r\n".format(self.value)
            return retVal

    class ZAccelOffsetRegister(Register):
        def __init__(self, reg_manager: RegisterManager):
            Register.__init__(self, reg_manager, 0x7d, 2, False)
            self.value = 0


        def read(self):
            self._manager.read(self)
            return self
            
        def get_value(self):
            return ((self.value & 0x7FFF) << 1)

        def set_value(self, value: int):
            self.value = ((value >> 1) & 0x7FFF)

        def __str__(self):
            retVal = ""
            retVal += "Value: {} (offset: 1, width: 15)\r\n".format(self.value)
            return retVal

