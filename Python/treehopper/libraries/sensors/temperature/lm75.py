from treehopper.api import I2C
from treehopper.libraries import SMBusDevice
from treehopper.libraries.sensors.temperature import TemperatureSensor
from treehopper.libraries.register_manager import sign_extend


class Lm75(TemperatureSensor):
    """LM75 I2C temperature sensor, made by Texas Instruments, STMicroelectronics, NXP, and Maxim"""
    def __init__(self, i2c: I2C, a0=False, a1=False, a2=False):
        super().__init__()
        self._dev = SMBusDevice(0x48 | (a0 << 0) | (a1 << 1) | (a2 << 2), i2c)

    def update(self):
        data = sign_extend(self._dev.read_word_data_be(0), 16)
        self._celsius = data / 32 / 8
