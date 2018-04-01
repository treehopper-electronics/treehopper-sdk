from treehopper.api import I2C
from treehopper.libraries import SMBusDevice
from treehopper.libraries.sensors.magnetic.Magnetometer import Magnetometer
from treehopper.libraries.sensors.magnetic.Ak8975Registers import Ak8975Registers


class Ak8975(Magnetometer):
    def __init__(self, i2c: I2C):
        super().__init__()
        dev = SMBusDevice(0x0c, i2c)
        self._registers = Ak8975Registers(dev)

    def update(self):
        self._registers.control.mode = 1
        self._registers.control.write()
        while True:
            self._registers.status1.read()
            if self._registers.status1.drdy:
                break

        self._registers.readRange(self._registers.hx, self._registers.hz)
        self._magnetometer = [self._registers.hx.value,
                             self._registers.hy.value,
                             self._registers.hz.value]
