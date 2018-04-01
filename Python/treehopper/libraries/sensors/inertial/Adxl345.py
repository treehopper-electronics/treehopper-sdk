from treehopper.api import I2C
from treehopper.libraries import SMBusDevice
from treehopper.libraries.sensors.inertial.Adxl345Registers import Adxl345Registers
from treehopper.libraries.sensors.inertial import Accelerometer


class Adxl345(Accelerometer):
    def __init__(self, i2c: I2C, alt_address=False, rate=100):
        super().__init__()
        self._dev = SMBusDevice((0x53 if alt_address else 0x1D), i2c, rate)
        self._registers = Adxl345Registers(self._dev)
        self._registers.powerCtl.sleep = 0
        self._registers.powerCtl.measure = 1
        self._registers.dataFormat.range = 0x03
        self._registers.dataFormat.fullRes = 1
        self._registers.write_range(self._registers.powerCtl, self._registers.dataFormat)

    def update(self):
        self._registers.read_range(self._registers.dataX, self._registers.dataZ)
        self._accelerometer = [self._registers.dataX.value / 255,
                              self._registers.dataY.value / 255,
                              self._registers.dataZ.value / 255]

