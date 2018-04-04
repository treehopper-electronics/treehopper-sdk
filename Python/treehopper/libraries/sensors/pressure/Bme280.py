from math import log
from typing import List

from treehopper.api import I2C
from treehopper.libraries import SMBusDevice
from treehopper.libraries.register_manager import sign_extend
from treehopper.libraries.sensors.humidity.humidity_sensor import HumiditySensor
from treehopper.libraries.sensors.pressure import Bmp280
from treehopper.libraries.sensors.pressure.bmp280_registers import Oversamplings


class Bme280(Bmp280, HumiditySensor):
    @staticmethod
    def probe(i2c: I2C) -> List['Bme280']:
        devs = []  # type: List['Bme280']
        try:
            dev = SMBusDevice(0x76, i2c, 100)
            who_am_i = dev.read_byte_data(0xD0)
            if who_am_i == 0x60:
                devs.append(Bme280(i2c, False))
        except RuntimeError:
            pass

        try:
            dev = SMBusDevice(0x76, i2c, 100)
            who_am_i = dev.read_byte_data(0xD0)
            if who_am_i == 0x60:
                devs.append(Bme280(i2c, True))
        except RuntimeError:
            pass

        return devs

    def __init__(self, i2c: I2C, sdo=False, rate=100.0):
        super().__init__(i2c, sdo, rate)
        self._registers.readRange(self._registers.h2, self._registers.h6)

        self._h4 = sign_extend(self._registers.h4.value << 4 | self._registers.h4h5.h4Low, 12)
        self._h5 = sign_extend(self._registers.h5.value << 4 | self._registers.h4h5.h5Low, 12)

        self._registers.ctrlHumidity.oversampling = Oversamplings.Oversampling_x16
        self._registers.ctrlHumidity.write()
        self._registers.ctrlMeasure.write()

    def update(self):
        super().update()

        var_h = self.t_fine - 76800.0
        var_h = (self._registers.humidity.value - (self._h4 * 64.0 + self._h5 / 16384.0 * var_h)) * \
                self._registers.h2.value / 65536.0 * (1.0 + self._registers.h6.value / 67108864.0 * var_h *
                                                      (1.0 + self._registers.h3.value / 67108864.0 * var_h))

        var_h = var_h * (1.0 - self._registers.h1.value * var_h / 524288.0)

        if var_h > 100.0:
            var_h = 100.0
        elif var_h < 0.0:
            var_h = 0.0

        self._relative_humidity = var_h
