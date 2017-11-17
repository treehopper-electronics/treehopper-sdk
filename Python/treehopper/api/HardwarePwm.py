import math
import logging
from treehopper.api.Pwm import Pwm
from treehopper.api.PinMode import PinMode
from treehopper.utils import utils


class HardwarePwm(Pwm):
    def __init__(self, pin):
        self._pin = pin
        self._board = pin._board
        self._duty_cycle = 0
        self._pulse_width = 0
        self._enabled = False
        self._logger = logging.getLogger(__name__)

    @property
    def enabled(self):
        return self._enabled

    @enabled.setter
    def enabled(self, value):
        if value == self._enabled:
            return

        if value:
            self._board.hardware_pwm_manager.start_pin(self._pin)
            self._pin.mode = PinMode.Reserved
        else:
            self._board.hardware_pwm_manager.stop_pin(self._pin)
            self._pin.mode = PinMode.Unassigned

    @property
    def duty_cycle(self):
        return self._duty_cycle

    @duty_cycle.setter
    def duty_cycle(self, value):
        if math.isclose(value, self._duty_cycle):
            return

        self._duty_cycle = value

        if value > 1.0 or value < 0.0:
            self._logger.warning("duty_cycle called with out-of-bounds value. Constraining to [0.0, 1.0]")
            self._duty_cycle = utils.constrain(self._duty_cycle)

        self._pulse_width = self._duty_cycle * self._board.hardware_pwm_manager.period_microseconds
        self._board.hardware_pwm_manager.set_duty_cycle(self._pin, self._duty_cycle)

    @property
    def pulse_width(self):
        return self._pulse_width

    @pulse_width.setter
    def pulse_width(self, value):
        if math.isclose(value, self._pulse_width):
            return

        self._duty_cycle = value

        if value > self._board.hardware_pwm_manager.period_microseconds or value < 0.0:
            self._logger.warning("pulse_width called with out-of-bounds value. Constraining to [0.0, {}]".format(self._board.hardware_pwm_manager.period_microseconds))
            self._duty_cycle = utils.constrain(self._duty_cycle)

        self.duty_cycle = self._pulse_width / self._board.hardware_pwm_manager.period_microseconds

    @property
    def enabled(self):
        return self._enabled

    @enabled.setter
    def enabled(self, value):
        if value == self._enabled:
            return

        self._enabled = value

        if self._enabled:
            self._board.hardware_pwm_manager.start_pin(self._pin)
            self._pin.mode = PinMode.Reserved
        else:
            self._board.hardware_pwm_manager.stop_pin(self._pin)
            self._pin.mode = PinMode.Unassigned

    def __str__(self):
        if self._enabled:
            return "Not enabled"
        else:
            return "{:0.2f}% duty cycle ({:0.02f} us pulse width)".format(self.duty_cycle * 100, self.pulse_width)
