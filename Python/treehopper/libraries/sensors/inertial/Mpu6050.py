from typing import List
from math import log2

from treehopper.api import I2c
from treehopper.libraries import SMBusDevice
from treehopper.libraries.sensors.inertial.Mpu6050Registers import Mpu6050Registers
from treehopper.libraries.sensors.inertial import Accelerometer, Gyroscope
from treehopper.libraries.sensors.temperature.Temperature import Temperature


class Mpu6050(Accelerometer, Gyroscope, Temperature):
    @staticmethod
    def probe(i2c: I2c) -> List['Mpu6050']:
        devs = []  # type: List['Mpu6050']
        try:
            dev = SMBusDevice(0x68, i2c, 100)
            who_am_i = dev.read_byte_data(0x68)
            devs.append(Mpu6050(i2c, False))
        except RuntimeError:
            pass

        try:
            dev = SMBusDevice(0x69, i2c, 100)
            who_am_i = dev.read_byte_data(0x68)
            devs.append(Mpu6050(i2c, False))
        except RuntimeError:
            pass

        return devs

    def __init__(self, i2c: I2c, alt_address=False, rate=100):
        super().__init__()
        self._accel_scale = 2
        self._dev = SMBusDevice((0x69 if alt_address else 0x68), i2c, rate)
        self._registers = Mpu6050Registers(self._dev)
        self._registers.powerMgmt1.reset = 1
        self._registers.powerMgmt1.write()
        self._registers.powerMgmt1.sleep = 0
        self._registers.powerMgmt1.write()
        self._registers.powerMgmt1.clockSel = 1
        self._registers.powerMgmt1.write()
        self._registers.configuration.dlpf = 3
        self._registers.configuration.write()
        self._registers.sampleRateDivider.value = 4
        self._registers.sampleRateDivider.write()
        self._registers.accelConfig2.read()
        self._registers.accelConfig2.accelFchoice = 0
        self._registers.accelConfig2.dlpfCfg = 3
        self._registers.accelConfig2.write()

    @property
    def accel_scale(self):
        return self._accel_scale

    @accel_scale.setter
    def accel_scale(self, value: int):
        if value != 2 and value != 4 and value != 8 and value != 16:
            raise ValueError("Accelerometer scale must be 2, 4, 8, or 16")
        self._accel_scale = value
        self._registers.accelConfig.accelFsSel = int(log2(value)-1)
        self._registers.accelConfig.write()

    def update(self):
        self._registers.read_range(self._registers.accel_x, self._registers.gyro_z)
        self._accelerometer = [self._registers.accel_x.value * self.accel_scale / 32768.0,
                               self._registers.accel_y.value * self.accel_scale / 32768.0,
                               self._registers.accel_z.value * self.accel_scale / 32768.0]

        self._gyroscope = [self._registers.gyro_x.value,
                           self._registers.gyro_y.value,
                           self._registers.gyro_z.value]

        self._celsius = self._registers.temp.value / 333.87 + 21.0