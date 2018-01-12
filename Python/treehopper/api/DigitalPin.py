from treehopper.utils.EventHandler import EventHandler
from abc import abstractmethod


class DigitalBase:
    def __init__(self):
        self._digital_value = False


class DigitalIn(DigitalBase):
    def __init__(self):
        super().__init__()
        self.digital_value_changed = EventHandler(self)

    @property
    def digital_value(self) -> bool:
        return self._digital_value

    @abstractmethod
    def make_digital_in(self):
        pass

    def _update_value(self, value: bool):
        if value == self._digital_value:
            return

        self._digital_value = value
        self.digital_value_changed(value)


class DigitalOut(DigitalBase):
    def __init__(self):
        super().__init__()

    @property
    def digital_value(self) -> bool:
        return self._digital_value

    @digital_value.setter
    @abstractmethod
    def digital_value(self, value):
        pass

    @abstractmethod
    def make_digital_push_pull_out(self):
        pass
