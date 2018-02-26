from time import sleep
from typing import List

from treehopper.api import I2c
from treehopper.libraries import SMBusDevice
from treehopper.libraries.sensors.inertial.Bno055Registers import Bno055Registers, OperatingModes, PowerModes
from treehopper.libraries.sensors.inertial import Accelerometer, Gyroscope
from treehopper.libraries.sensors.magnetic.Magnetometer import Magnetometer
from treehopper.libraries.sensors.temperature import TemperatureSensor


class Bno055(Accelerometer, Gyroscope, Magnetometer, TemperatureSensor):
    @staticmethod
    def probe(i2c: I2c, rate=100) -> List['Bno055']:
        devs = []  # type: List['Bno055']
        try:
            dev = SMBusDevice(0x28, i2c, rate)
            who_am_i = dev.read_byte_data(0x00)
            if who_am_i == 0xa0:
                devs.append(Bno055(i2c, False, rate))
        except RuntimeError:
            pass

        try:
            dev = SMBusDevice(0x29, i2c, rate)
            who_am_i = dev.read_byte_data(0x00)
            if who_am_i == 0xa0:
                devs.append(Bno055(i2c, False, rate))
        except RuntimeError:
            pass

        return devs

    def __init__(self, i2c: I2c, alt_address=False, rate=100):
        super().__init__()
        self._linear_acceleration = [0, 0, 0]
        self._quaternion = [0, 0, 0, 0]
        self._gravity = [0, 0, 0]
        self._eular_angles = [0, 0, 0]

        if alt_address:
            dev = SMBusDevice(0x29, i2c, rate)
        else:
            dev = SMBusDevice(0x28, i2c, rate)

        self._registers = Bno055Registers(dev)

        self._registers.operatingMode.operatingMode = OperatingModes.ConfigMode
        self._registers.operatingMode.write()
        self._registers.sysTrigger.resetSys = 1
        self._registers.sysTrigger.write()
        self._registers.sysTrigger.resetSys = 0
        dev_id = 0
        while dev_id != 0xA0:
            try:
                self._registers.chipId.read()
                dev_id = self._registers.chipId.value
            except RuntimeError:
                pass

        sleep(0.05)
        self._registers.powerMode.powerMode = PowerModes.Normal
        self._registers.powerMode.write()
        sleep(0.01)
        self._registers.sysTrigger.selfTest = 0
        self._registers.sysTrigger.write()
        sleep(0.01)
        self._registers.operatingMode.operatingMode = OperatingModes.NineDegreesOfFreedom
        self._registers.operatingMode.write()
        sleep(0.02)

    @property
    def linear_acceleration(self):
        if self.auto_update_when_property_read:
            self.update()

        return self._linear_acceleration

    @property
    def gravity(self):
        if self.auto_update_when_property_read:
            self.update()

        return self._gravity

    @property
    def eular_angles(self):
        if self.auto_update_when_property_read:
            self.update()

        return self._eular_angles

    @property
    def quaternion(self):
        if self.auto_update_when_property_read:
            self.update()

        return self._quaternion

    def update(self):
        self._registers.readRange(self._registers.accelX, self._registers.temp)

        self._accelerometer = [self._registers.accelX.value / 16,
                               self._registers.accelY.value / 16,
                               self._registers.accelZ.value / 16]

        self._magnetometer = [self._registers.magnetometerX.value / 16,
                              self._registers.magnetometerY.value / 16,
                              self._registers.magnetometerZ.value / 16]

        self._gyroscope = [self._registers.gyroX.value / 16,
                           self._registers.gyroY.value / 16,
                           self._registers.gyroZ.value / 16]

        self._linear_acceleration = [self._registers.linX.value / 100,
                                     self._registers.linY.value / 100,
                                     self._registers.linZ.value / 100]

        self._gravity = [self._registers.gravX.value / 100,
                         self._registers.gravY.value / 100,
                         self._registers.gravZ.value / 100]

        self._eular_angles = [self._registers.eulPitch.value / 100,
                              self._registers.eulRoll.value / 100,
                              self._registers.eulHeading.value / 100]

        self._quaternion = [self._registers.quaW.value / 16384,
                            self._registers.quaX.value / 16384,
                            self._registers.quaY.value / 16384,
                            self._registers.quaZ.value / 16384]

        self._celsius = self._registers.temp.value