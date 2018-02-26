from math import log
from typing import List

from treehopper.api import I2c
from treehopper.libraries import SMBusDevice
from treehopper.libraries.sensors.pressure.Pressure import Pressure
from treehopper.libraries.sensors.pressure.Bmp280Registers import Bmp280Registers, Modes, OversamplingPressures, \
    OversamplingTemperatures
from treehopper.libraries.sensors.temperature import TemperatureSensor


class Bmp280(Pressure, TemperatureSensor):
    @staticmethod
    def probe(i2c: I2c, include_bme280=True, rate=100.0) -> List['Bmp280']:
        devs = []  # type: List['Bmp280']
        try:
            dev = SMBusDevice(0x76, i2c, 100)
            who_am_i = dev.read_byte_data(0xD0)
            if who_am_i == 0x58 or (who_am_i == 0x60 and include_bme280):
                devs.append(Bmp280(i2c, False, rate))
        except RuntimeError:
            pass

        try:
            dev = SMBusDevice(0x76, i2c, 100)
            who_am_i = dev.read_byte_data(0xD0)
            if who_am_i == 0x58 or (who_am_i == 0x60 and include_bme280):
                devs.append(Bmp280(i2c, True, rate))
        except RuntimeError:
            pass

        return devs

    def __init__(self, i2c: I2c, sdo=False, rate=100.0):
        super().__init__()
        self._registers = Bmp280Registers(SMBusDevice(0x76 if sdo is False else 0x77, i2c, rate))

        self._registers.ctrlMeasure.mode = Modes.Normal
        self._registers.ctrlMeasure.oversamplingPressure = OversamplingPressures.Oversampling_x16
        self._registers.ctrlMeasure.oversamplingTemperature = OversamplingTemperatures.Oversampling_x16
        self._registers.ctrlMeasure.write()

        self._registers.readRange(self._registers.t1, self._registers.h1)
        self.t_fine = 0
        self._altitude = 0
        self.reference_pressure = 101325.0

    @property
    def altitude(self):
        return self._altitude

    def update(self):
        self._registers.readRange(self._registers.pressure, self._registers.humidity)

        var1 = (self._registers.temperature.value / 16384.0 - self._registers.t1.value / 1024.0) \
               * self._registers.t2.value
        var2 = ((self._registers.temperature.value / 131072.0 - self._registers.t1.value / 8192.0) *
                (self._registers.temperature.value / 131072.0 - self._registers.t1.value / 8192.0)) * \
               self._registers.t3.value

        self.t_fine = var1 + var2
        self._celsius = (var1 + var2) / 5120.0

        var1 = self.t_fine / 2.0 - 64000.0
        var2 = var1 * var1 * self._registers.p6.value / 32768.0
        var2 = var2 + var1 * self._registers.p5.value * 2.0
        var2 = (var2 / 4.0) + self._registers.p4.value * 65536.0
        var1 = (self._registers.p3.value * var1 * var1 / 524288.0 + self._registers.p2.value * var1) / 524288.0
        var1 = (1.0 + var1 / 32768.0) * self._registers.p1.value

        if var1 != 0:
            pressure = 1048576.0 - self._registers.pressure.value
            pressure = (pressure - (var2 / 4096.0)) * 6250.0 / var1
            var1 = self._registers.p9.value * pressure * pressure / 2147483648.0
            var2 = pressure * self._registers.p8.value / 32768.0
            pressure = pressure + (var1 + var2 + self._registers.p7.value) / 16.0
            self._pascal = pressure

            # calculate altitude
            M = 0.0289644
            g = 9.80665
            R = 8.31432

            if self.reference_pressure / pressure < 101325 / 22632.1:
                d = -0.0065
                e = 0
                j = pow(pressure / self.reference_pressure, R * d / (g * M))
                self._altitude = e + self.kelvin * (1 / j - 1) / d

            elif self.reference_pressure / pressure < 101325 / 5474.89:
                e = 11000
                b = self.kelvin - 71.5
                f = R * b * log(pressure / self.reference_pressure) / (-g * M)
                l = 101325
                c = 22632.1
                h = R * b * log(l / c) / (-g * M) + e
                self._altitude = h + f
