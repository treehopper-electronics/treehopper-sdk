from abc import abstractmethod
from treehopper.libraries.sensors import Pollable


class Pressure(Pollable):
    def __init__(self):
        super().__init__()
        self._pascal = 0

    @property
    def pascal(self):
        if self.auto_update_when_property_read:
            self.update()

        return self._pascal

    @property
    def bar(self):
        return self.pascal / 100000.0

    @property
    def atm(self):
        return self.pascal / (1.01325 * 100000.0)

    @property
    def psi(self):
        return self.atm / 14.7

    @abstractmethod
    def update(self):
        pass