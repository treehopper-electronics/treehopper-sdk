from abc import abstractmethod
from treehopper.libraries.sensors import Pollable


class Magnetometer(Pollable):
    def __init__(self):
        super().__init__()
        self._magnetometer = [0.0, 0.0, 0.0]

    @property
    def magnetometer(self):
        if self.auto_update_when_property_read:
            self.update()

        return self._magnetometer

    @abstractmethod
    def update(self):
        pass