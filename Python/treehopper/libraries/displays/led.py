import logging
import math
from treehopper.libraries.flushable import Flushable
import abc
from treehopper.utils.utils import constrain


class Led:
    """A single, monochromatic LED

    Attributes:
        brightness_control: whether this LED has individual brightness control
        channel: an integer representing the channel of the driver to which this LED belongs
        driver: the LedDriver instance that owns this LED

    """
    def __init__(self, driver: 'LedDriver', channel=0, has_brightness_control=False):
        self._brightness = 1.0
        self._state = False
        self.brightness_control = has_brightness_control
        self.channel = channel
        self.driver = driver

    @property
    def brightness(self, value):
        """Gets or sets the brightness of the LED (from 0.0 - 1.0)"""
        if math.isclose(value, self._brightness):
            return

        if not self.brightness_control:
            return

        self._brightness = constrain(value, 0.0, 1.0)
        self.driver.led_brightness_changed(self)

    @brightness.getter
    def brightness(self):
        return self._brightness

    @property
    def state(self):
        """Gets or sets the state of the LED (True or False)"""
        return self._state

    @state.setter
    def state(self, value):
        if self._state == value:
            return

        self._state = value
        self.driver._led_state_changed(self)

    def __str__(self):
        return "{0} ({1})".format(self._state, self._brightness)


class LedDriver(Flushable):
    """An LED driver, controlling a collection of one or more LEDs

    Attributes:
        leds: the collection of LEDs controlled by this driver.
        individual_brightness: whether this controller has individual brightness control
        global_brightness: whether this controller has global brightness control

    """
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

    def clear(self):
        autoflush_save = self.auto_flush
        self.auto_flush = False
        for led in self.leds:
            led.state = False
        self.flush(True)
        self.auto_flush = autoflush_save


