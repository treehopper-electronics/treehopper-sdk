from typing import List

from treehopper.api import I2c
from treehopper.libraries import SMBusDevice
from treehopper.libraries.sensors.temperature import TemperatureSensor
from treehopper.libraries.Register import sign_extend


class Mcp9808(TemperatureSensor):
    @staticmethod
    def probe(i2c: I2c, rate=100) -> List['Mcp9808']:
        devs = []  # type: List['Mcp9808']
        try:
            dev = SMBusDevice(0x18, i2c, rate)
            who_am_i = dev.read_byte_data(0x07)
            if who_am_i == 0x04:
                devs.append(Mcp9808(i2c, False, False, False))
        except RuntimeError:
            pass

        try:
            dev = SMBusDevice(0x19, i2c, rate)
            who_am_i = dev.read_byte_data(0x07)
            if who_am_i == 0x04:
                devs.append(Mcp9808(i2c, True, False, False))
        except RuntimeError:
            pass

        try:
            dev = SMBusDevice(0x1a, i2c, rate)
            who_am_i = dev.read_byte_data(0x07)
            if who_am_i == 0x04:
                devs.append(Mcp9808(i2c, False, True, False))
        except RuntimeError:
            pass

        try:
            dev = SMBusDevice(0x1b, i2c, rate)
            who_am_i = dev.read_byte_data(0x07)
            if who_am_i == 0x04:
                devs.append(Mcp9808(i2c, True, True, False))
        except RuntimeError:
            pass

        try:
            dev = SMBusDevice(0x1c, i2c, rate)
            who_am_i = dev.read_byte_data(0x07)
            if who_am_i == 0x04:
                devs.append(Mcp9808(i2c, False, False, True))
        except RuntimeError:
            pass

        try:
            dev = SMBusDevice(0x1d, i2c, rate)
            who_am_i = dev.read_byte_data(0x07)
            if who_am_i == 0x04:
                devs.append(Mcp9808(i2c, True, False, True))
        except RuntimeError:
            pass

        try:
            dev = SMBusDevice(0x1e, i2c, rate)
            who_am_i = dev.read_byte_data(0x07)
            if who_am_i == 0x04:
                devs.append(Mcp9808(i2c, False, True, True))
        except RuntimeError:
            pass

        try:
            dev = SMBusDevice(0x1f, i2c, rate)
            who_am_i = dev.read_byte_data(0x07)
            if who_am_i == 0x04:
                devs.append(Mcp9808(i2c, True, True, True))
        except RuntimeError:
            pass

        return devs

    def __init__(self, i2c: I2c, a0=False, a1=False, a2=False):
        super().__init__()
        self._dev = SMBusDevice(0x18 | (a0 << 0) | (a1 << 1) | (a2 << 2), i2c)

    def update(self):
        data = sign_extend(self._dev.read_word_data_be(0x05), 13)
        self._celsius = data / 16
