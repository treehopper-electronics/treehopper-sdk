import logging
import math
from treehopper.libraries.Flushable import Flushable
from treehopper.libraries.displays.Led import Led
import abc
from treehopper.utils.utils import constrain


class LedDriver(Flushable):
    def __init__(self, num_leds: int, has_global_brightness_control: bool, has_individual_brightness_control: bool):
        Flushable.__init__(self)
        self._logger = logging.getLogger(__name__)
        self._brightness = 1.0
        self.global_brightness = has_global_brightness_control
        self.individual_brightness = has_individual_brightness_control
        self.leds = []
        for i in range(num_leds):
            self.leds.append(Led(self, i, self.individual_brightness))

    @abc.abstractmethod
    def flush(self, force=False):
        pass

    @abc.abstractmethod
    def _led_state_changed(self, led: Led):
        pass

    @abc.abstractmethod
    def _led_brightness_changed(self, led: Led):
        pass

    @abc.abstractmethod
    def _set_global_brightness(self, value: float):
        pass

    @property
    def brightness(self, value):
        if math.isclose(value, self.brightness):
            return

        if value < 0.0 or value > 1.0:
            self._logger.warning("Constraining out-of-range brightness value to 0.0-1.0")

        self._brightness = constrain(value, 0.0, 1.0)

        if self.global_brightness:
            self._set_global_brightness(self._brightness)
        else:
            saved_autoflush = self.auto_flush
            self.auto_flush = False
            for led in self.leds:
                led.brightness = self._brightness

            self.flush()
            self.auto_flush = saved_autoflush


    @brightness.getter
    def brightness(self):
        return self._brightness
