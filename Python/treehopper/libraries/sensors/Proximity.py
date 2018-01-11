from abc import abstractmethod, ABC
from treehopper.libraries.sensors import Pollable


class Proximity(Pollable):
    def __init__(self):
        self._meters = 0

    @property
    def meters(self):
        if self.auto_update_when_property_read:
            self.update()

        return self._meters

    @property
    def centimeters(self):
        return self.meters * 100

    @property
    def inches(self):
        return self.meters * 39.3701

    @property
    def feet(self):
        return self.meters * 3.28085
