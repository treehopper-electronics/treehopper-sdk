from treehopper.api import I2c
from treehopper.libraries import SMBusDevice
from treehopper.libraries.sensors.temperature import TemperatureSensor


class TempRegister(TemperatureSensor):
    def __init__(self, dev: SMBusDevice, register: int):
        self._dev = dev
        self._register = register

    def update(self):
        data = self._dev.read_word_data(self._register)
        data &= 0x7FFF
        self._celsius = data * 0.02 - 273.15


class Mlx90614:
    def __init__(self, i2c: I2c):
        self._dev = SMBusDevice(0x5a, i2c)
        self.ambient = TempRegister(self._dev, 0x06)
        self.object = TempRegister(self._dev, 0x07)


class Mlx90615:
    def __init__(self, i2c: I2c):
        self._dev = SMBusDevice(0x5b, i2c)
        self.ambient = TempRegister(self._dev, 0x26)
        self.object = TempRegister(self._dev, 0x27)
