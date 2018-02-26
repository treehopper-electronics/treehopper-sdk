from treehopper.libraries import SMBusDevice
from treehopper.libraries.sensors.optical.Vcnl4010Registers import Vcnl4010Registers
from treehopper.libraries.sensors.optical.Vcnl4010Registers import Rates, AlsRates
from treehopper.api import I2c
from treehopper.libraries.sensors.optical.AmbientLightSensor import AmbientLightSensor
from treehopper.libraries.sensors import Proximity


class Vcnl4010(AmbientLightSensor, Proximity):
    """Vishay VCNL4010 proximity and ambient light sensor."""
    def __init__(self, i2c: I2c):
        self.registers = Vcnl4010Registers(SMBusDevice(0x13, i2c))
        self.registers.readRange(self.registers.command, self.registers.ambientLightParameters)
        self.registers.proximityRate.set_value(Rates.Hz_7_8125)
        self.registers.ledCurrent.irLedCurrentValue = 20
        self.registers.ambientLightParameters.set_value(AlsRates.Hz_10)
        self.registers.ambientLightParameters.autoOffsetCompensation = 1
        self.registers.ambientLightParameters.averagingSamples = 5
        self.registers.writeRange(self.registers.command, self.registers.ambientLightParameters)
        self._raw_proximity = 0

    def update(self):
        self.registers.command.alsOnDemandStart = 1
        self.registers.command.proxOnDemandStart = 1
        self.registers.command.write()

        while True:
            self.registers.command.read()
            if self.registers.command.proxDataReady == 1 and self.registers.command.alsDataReady == 1:
                break

        self.registers.ambientLightResult.read()
        self.registers.proximityResult.read()

        self._lux = self.registers.ambientLightResult.value * 0.25
        self._raw_proximity = self.registers.proximityResult.value

        if self.registers.proximityResult.value < 2295:
            self._meters = float('+inf')
        else:
            self._meters = 81.0 * pow(self.registers.proximityResult.value - 2298.0, -0.475) / 100.0

    @property
    def raw_proximity(self):
        if self.auto_update_when_property_read:
            self.update()

        return self._raw_proximity

    @property
    def led_current(self):
        return self.registers.ledCurrent.irLedCurrentValue * 10

    @led_current.setter
    def led_current(self, value):
        self.registers.ledCurrent.irLedCurrentValue = round(value / 10)
        self.registers.ledCurrent.write()
