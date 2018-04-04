from typing import List

from treehopper.api import I2C
from treehopper.libraries import SMBusDevice
from treehopper.libraries.sensors.inertial import Mpu6050, Gyroscope
from treehopper.libraries.sensors.magnetic.ak8975 import Ak8975
from treehopper.libraries.sensors.magnetic.magnetometer import Magnetometer


class Mpu9250(Mpu6050, Magnetometer):
    @staticmethod
    def probe(i2c: I2C) -> List['Mpu9250']:
        devs = []  # type: List['Mpu9250']
        try:
            dev = SMBusDevice(0x68, i2c, 100)
            who_am_i = dev.read_byte_data(0x75)
            if who_am_i == 0x71:
                devs.append(Mpu9250(i2c, False))
        except RuntimeError:
            pass

        try:
            dev = SMBusDevice(0x69, i2c, 100)
            who_am_i = dev.read_byte_data(0x75)
            if who_am_i == 0x71:
                devs.append(Mpu9250(i2c, True))
        except RuntimeError:
            pass

        return devs

    def __init__(self, i2c: I2C, alt_address=False, rate=100):
        super().__init__(i2c, alt_address, rate)
        self._registers.intPinCfg.bypassEn = 1
        self._registers.intPinCfg.latchIntEn = 1
        self._registers.intPinCfg.write()
        self._mag = Ak8975(i2c)
        self._mag.auto_update_when_property_read = False
        self.enable_magnetometer = True

    def update(self):
        super().update()
        if self.enable_magnetometer:
            self._mag.update()
            self._magnetometer = self._mag.magnetometer
