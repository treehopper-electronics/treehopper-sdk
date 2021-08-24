from time import sleep

from treehopper.libraries.sensors.temperature.temperature_sensor import TemperatureSensor
from treehopper.api.uart import OneWire


class Ds18b20(TemperatureSensor):
    """Maxim DS18B20 1-Wire temperature sensor

    Attributes:
        enable_group_conversion: whether to request a conversion from all sensors on the bus simultaneously
        address: the address of the sensor

    Example:
        >>> from time import sleep
        >>> from treehopper.api import *
        >>> from treehopper.libraries.sensors.temperature import Ds18b20

        >>> board = find_boards()[0]
        >>> board.connect()
        >>> sensor = Ds18b20(board.uart)
        >>> sensor.auto_update_when_property_read = False
        >>> while board.connected:
        >>>     sensor.update()
        >>>     print("Temperature: {} °C ({} °F)".format(sensor.celsius, sensor.fahrenheit))
        >>>     sleep(0.1)

    """
    def __init__(self, one_wire: OneWire, address=0):
        super().__init__()
        self._bus = one_wire
        one_wire.start_one_wire()
        self.address = address
        self.enable_group_conversion = False

    def update(self):
        if not self.enable_group_conversion:
            if self.address == 0:
                self._bus.one_wire_reset()
                self._bus.send([0xCC, 0x44])
            else:
                self._bus.one_wire_reset_and_match_address(self.address)
                self._bus.send(0x44)

            sleep(0.750)

        if self.address == 0:
            self._bus.one_wire_reset()
            self._bus.send([0xCC, 0xBE])
        else:
            self._bus.one_wire_reset_and_match_address(self.address)
            self._bus.send(0xBE)

        data = self._bus.receive(2)

        self._celsius = int.from_bytes(data, 'little') / 16.0
