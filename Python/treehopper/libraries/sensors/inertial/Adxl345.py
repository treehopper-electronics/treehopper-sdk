from treehopper.api import I2C
from treehopper.libraries import SMBusDevice
from treehopper.libraries.sensors.inertial.adxl345_registers import Adxl345Registers
from treehopper.libraries.sensors.inertial import Accelerometer


class Adxl345(Accelerometer):
    """Analog Devices ADXL-345 three-axis I2C accelerometer

    Example:
        >>> from treehopper.api import *
        >>> from treehopper.libraries.sensors.inertial import Adxl345

        >>> board = find_boards()[0]
        >>> board.connect()
        >>> imu = Adxl345(alt_address=True, i2c=board.i2c)
        >>> while board.connected:
        >>>     print(imu.accelerometer)

    """
    def __init__(self, i2c: I2C, alt_address=False, rate=100):
        super().__init__()
        self._dev = SMBusDevice((0x53 if alt_address else 0x1D), i2c, rate)
        self._registers = Adxl345Registers(self._dev)
        self._registers.powerCtl.sleep = 0
        self._registers.powerCtl.measure = 1
        self._registers.dataFormat.range = 0x03
        self._registers.dataFormat.fullRes = 1
        self._registers.writeRange(self._registers.powerCtl, self._registers.dataFormat)

    def update(self):
        self._registers.readRange(self._registers.dataX, self._registers.dataZ)
        self._accelerometer = [self._registers.dataX.value / 255,
                              self._registers.dataY.value / 255,
                              self._registers.dataZ.value / 255]

