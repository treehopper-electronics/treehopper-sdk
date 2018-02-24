from abc import abstractmethod
from treehopper.libraries.sensors import Pollable


class HumiditySensor(Pollable):
    def __init__(self):
        super().__init__()
        self._relative_humidity = 0.0

    @property
    def relative_humidity(self):
        if self.auto_update_when_property_read:
            self.update()

        return self._relative_humidity

    @abstractmethod
    def update(self):
        pass