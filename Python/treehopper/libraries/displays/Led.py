import math
from treehopper.utils.utils import constrain

class Led:
    def __init__(self, driver, channel=0, has_brightness_control=False):
        self._brightness = 1.0
        self._state = False
        self.brightness_control = has_brightness_control
        self.channel = channel
        self.driver = driver

    @property
    def brightness(self, value):
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
        return self._state

    @state.setter
    def state(self, value):
        if self._state == value:
            return

        self._state = value
        self.driver._led_state_changed(self)

    def __str__(self):
        return "{0} ({1})".format(self._state, self._brightness)
