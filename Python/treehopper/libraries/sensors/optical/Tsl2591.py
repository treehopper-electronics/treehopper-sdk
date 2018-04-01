from treehopper.api import I2C
from treehopper.libraries import SMBusDevice
from treehopper.libraries.sensors.optical import AmbientLightSensor
from treehopper.libraries.sensors.optical.Tsl2591Registers import Tsl2591Registers, AlsGains, AlsTimes


class Tsl2591(AmbientLightSensor):
    @staticmethod
    def probe(i2c: I2C):
        dev = SMBusDevice(0x29, i2c, 100)
        result = dev.read_byte_data(0xB2)
        if result == 0x50:
            return Tsl2591(i2c)

        return None

    class Gain:
        Low = 1
        Medium = 25
        High = 428
        Max = 9876

    def __init__(self, i2c):
        super().__init__()
        self._registers = Tsl2591Registers(SMBusDevice(0x29, i2c, 100))
        self._registers.enable.powerOn = 1
        self._registers.enable.alsEnable = 1
        self._registers.enable.write()

    @property
    def integration_time(self):
        return (self._registers.config.alsTime + 1) * 100.0

    @integration_time.setter
    def integration_time(self, value):
        self._registers.config.alsTime = value//100 - 1
        self._registers.config.write()

    @property
    def gain(self):
        if self._registers.config.alsGain == AlsGains.Low:
            return 1
        if self._registers.config.alsGain == AlsGains.Medium:
            return 25
        if self._registers.config.alsGain == AlsGains.High:
            return 428
        if self._registers.config.alsGain == AlsGains.Max:
            return 9876

    @gain.setter
    def gain(self, value):
        if value == Tsl2591.Gain.Low:
            self._registers.config.alsGain = 0
        if value == Tsl2591.Gain.Medium:
            self._registers.config.alsGain = 1
        if value == Tsl2591.Gain.High:
            self._registers.config.alsGain = 2
        if value == Tsl2591.Gain.Max:
            self._registers.config.alsGain = 3

        self._registers.config.write()

    def update(self):
        self._registers.readRange(self._registers.ch0, self._registers.ch1)
        if self._registers.ch0.value == 0 or self._registers.ch1.value == 0:
            return

        cpl = (self.integration_time * self.gain) / 408.0
        self._lux = (self._registers.ch0.value - self._registers.ch1.value) * \
                    (1.0 - (self._registers.ch1.value / self._registers.ch0.value)) / cpl
