from abc import abstractmethod
from treehopper.libraries.sensors.pollable import Pollable


class TemperatureSensor(Pollable):
    """Temperature sensor"""
    def __init__(self):
        super().__init__()
        self._celsius = 0

    @abstractmethod
    def update(self):
        """Request an update from the temperature sensor"""
        pass

    @property
    def celsius(self) -> float:
        """Gets the temperature, in degrees Celsius"""
        if self.auto_update_when_property_read:
            self.update()

        return self._celsius

    @property
    def fahrenheit(self):
        """Gets the temperature, in degrees Fahrenheit"""
        return self.celsius * 9.0 / 5.0 + 32.0

    @property
    def kelvin(self):
        """Gets the temperature, in Kelvin"""
        return self.celsius + 273.15
