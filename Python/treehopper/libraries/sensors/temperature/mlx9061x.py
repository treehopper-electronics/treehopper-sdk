from treehopper.api import I2C
from treehopper.libraries import SMBusDevice
from treehopper.libraries.sensors.temperature import TemperatureSensor


## \cond PRIVATE
class TempRegister(TemperatureSensor):
    def __init__(self, dev: SMBusDevice, register: int):
        super().__init__()
        self._dev = dev
        self._register = register

    def update(self):
        data = self._dev.read_word_data(self._register)
        data &= 0x7FFF
        self._celsius = data * 0.02 - 273.15
## \endcond

class Mlx90614:
    """Melexis MLX90614 non-contact I2C thermal sensor"""
    def __init__(self, i2c: I2C):
        self._dev = SMBusDevice(0x5a, i2c)
        self.ambient = TempRegister(self._dev, 0x06)
        self.object = TempRegister(self._dev, 0x07)


class Mlx90615:
    """Melexis MLX90615 non-contact I2C thermal sensor"""
    def __init__(self, i2c: I2C):
        self._dev = SMBusDevice(0x5b, i2c)
        self.ambient = TempRegister(self._dev, 0x26)
        self.object = TempRegister(self._dev, 0x27)
