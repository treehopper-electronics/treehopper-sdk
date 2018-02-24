from abc import abstractmethod
from treehopper.libraries.sensors import Pollable


class Gyroscope(Pollable):
    def __init__(self):
        super().__init__()
        self._gyroscope = [0.0, 0.0, 0.0]

    @property
    def gyroscope(self):
        if self.auto_update_when_property_read:
            self.update()

        return self._gyroscope

    @abstractmethod
    def update(self):
        pass