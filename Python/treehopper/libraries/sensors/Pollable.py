from abc import abstractmethod


class Pollable:
    def __init__(self):
        self.auto_update_when_property_read = True

    @abstractmethod
    def update(self):
        pass
    