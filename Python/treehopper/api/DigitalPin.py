from treehopper.utils.EventHandler import EventHandler


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

    def make_digital_in(self):
        pass


class DigitalOut(DigitalBase):
    def __init__(self):
        super().__init__()

    @property
    def digital_value(self) -> bool:
        return self._digital_value

    @digital_value.setter
    def digital_value(self, value):
        pass

    def make_digital_out(self):
        pass
