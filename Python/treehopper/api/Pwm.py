from abc import ABC, abstractmethod


class Pwm(ABC):
    @property
    @abstractmethod
    def duty_cycle(self):
        pass

    @duty_cycle.setter
    @abstractmethod
    def duty_cycle(self, value):
        pass

    @property
    @abstractmethod
    def pulse_width(self):
        pass

    @pulse_width.setter
    @abstractmethod
    def duty_cycle(self, value):
        pass

    @property
    @abstractmethod
    def enabled(self):
        pass

    @enabled.setter
    @abstractmethod
    def enabled(self, value):
        pass



