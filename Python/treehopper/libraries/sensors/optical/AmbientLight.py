from abc import abstractmethod
from treehopper.libraries.sensors import Pollable


class AmbientLight(Pollable):
    def __init__(self):
        super().__init__()
        self._lux = 0

    @property
    def lux(self):
        if self.auto_update_when_property_read:
            self.update()

        return self._lux

    @abstractmethod
    def update(self):
        pass