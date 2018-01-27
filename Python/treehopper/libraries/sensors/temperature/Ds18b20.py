from time import sleep

from treehopper.libraries.sensors.temperature.Temperature import Temperature
from treehopper.api.IOneWire import IOneWire

class Ds18b20(Temperature):
    def __init__(self, one_wire: IOneWire, address=0):
        super().__init__()
        self.bus = one_wire
        one_wire.start_one_wire()
        self.address = address
        self.enable_group_conversion = False

    def update(self):
        if not self.enable_group_conversion:
            if self.address == 0:
                self.bus.one_wire_reset()
                self.bus.send([0xCC, 0x44])
            else:
                self.bus.one_wire_reset_and_match_address(self.address)
                self.bus.send(0x44)

            sleep(0.750)

        if self.address == 0:
            self.bus.one_wire_reset()
            self.bus.send([0xCC, 0xBE])
        else:
            self.bus.one_wire_reset_and_match_address(self.address)
            self.bus.send(0xBE)

        data = self.bus.receive(2)

        self._celsius = int.from_bytes(data, 'little') / 16.0
