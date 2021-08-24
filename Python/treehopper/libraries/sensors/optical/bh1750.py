from treehopper.api import I2C
from treehopper.libraries import SMBusDevice
from treehopper.libraries.sensors.optical import AmbientLightSensor


class Bh1750(AmbientLightSensor):
    """Rohm Semiconductor BH1750FVI ambient light sensor """
    class Resolution:
        Medium, High, Low = range(3)

    def __init__(self, i2c: I2C, address_pin=False):
        super().__init__()
        self._resolution = 0
        self._dev = SMBusDevice(0x5c if address_pin else 0x23, i2c, 100)
        self._dev.write_byte(0x07)  # reset
        self.resolution = Bh1750.Resolution.High

    @property
    def resolution(self):
        return self._resolution

    @resolution.setter
    def resolution(self, value):
        if self._resolution == value:
            return

        self._resolution = value
        self._dev.write_byte(0x10 | self._resolution)

    def update(self):
        self._lux = self._dev.read_word_be() / 1.2