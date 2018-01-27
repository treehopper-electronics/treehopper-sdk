from abc import abstractmethod
from treehopper.libraries.sensors.Pollable import Pollable

class Temperature(Pollable):
    def __init__(self):
        super().__init__()
        self._celsius = 0

    @abstractmethod
    def update(self):
        pass

    @property
    def celsius(self) -> float:
        if self.auto_update_when_property_read:
            self.update()

        return self._celsius

    @property
    def fahrenheit(self):
        return self.celsius * 9.0 / 5.0 + 32.0

    @property
    def kelvin(self):
        return self.celsius + 273.15
