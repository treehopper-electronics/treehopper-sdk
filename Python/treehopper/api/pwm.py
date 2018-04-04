import math
import logging

from treehopper.api import DeviceCommands
from treehopper.api.interfaces import Pwm
from treehopper.api.pin import Pin, PinMode
from treehopper.utils import utils


class HardwarePwm(Pwm):
    """Hardware PWM module."""

    ## \cond PRIVATE
    def __init__(self, pin: 'Pin'):
        self._pin = pin
        self._board = pin._board
        self._duty_cycle = 0
        self._pulse_width = 0
        self._enabled = False
        self._logger = logging.getLogger(__name__)
    ## \endcond

    def enable_pwm(self):
        """ Enable the PWM functionality of this pin. """
        self.enabled = True

    @property
    def enabled(self):
        """ Gets or sets whether the PWM channel is enabled. """
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
        """ Gets or sets the duty cycle of the PWM pin, from 0.0-1.0. """
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
        """ Gets or sets the pulse width, in ms, of the pin. """
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
        """ Enable or disable this hardware PWM pin. """
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


## \cond PRIVATE
class PwmPinEnableMode:
    NoPin, Pin7, Pin7Pin8, Pin7Pin8Pin9 = range(4)
## \endcond


class HardwarePwmFrequency:
    """ Enumeration of possible hardware PWM frequencies """
    Freq_732Hz, Freq_183Hz, Freq_61Hz = range(3)


class HardwarePwmManager:
    """Manages hardware PWM pins"""
    ## \cond PRIVATE
    def __init__(self, board):
        self._board = board
        self._duty_cycle_pin7 = [0, 0]
        self._duty_cycle_pin8 = [0, 0]
        self._duty_cycle_pin9 = [0, 0]
        self._frequency = HardwarePwmFrequency.Freq_732Hz
        self._mode = PwmPinEnableMode.NoPin
        self.logger = logging.getLogger(__name__)
    ## \endcond

    @property
    def microseconds_per_tick(self) -> float:
        """ Gets the microseconds per PWM tick. """
        return 1000000.0 / (self.frequency_hz * 65536)

    @property
    def period_microseconds(self) -> float:
        """ Gets the period, in microseconds, the PWM module is operating at. """
        return 1000000 / self.frequency_hz

    @property
    def frequency_hz(self) -> int:
        """ Gets the frequency, in hertz, of the PWM module. """
        if self._frequency == HardwarePwmFrequency.Freq_61Hz:
            return 61
        elif self._frequency == HardwarePwmFrequency.Freq_183Hz:
            return 183
        elif self._frequency == HardwarePwmFrequency.Freq_732Hz:
            return 732
        return 0

    def start_pin(self, pin: Pin):
        if pin.number == 8 and self._mode != PwmPinEnableMode.Pin7:
            self.logger.error(
                "You must enable PWM functionality on Pin 7 (PWM1) before you enable PWM functionality on Pin 8 (PWM2).")
        if pin.number == 9 and self._mode != PwmPinEnableMode.Pin7Pin8:
            self.logger.error(
                "You must enable PWM functionality on Pin 7 and 8 (PWM1 and PWM2) before you enable PWM functionality on Pin 9 (PWM3).")

        if pin.number == 7:
            self._mode = PwmPinEnableMode.Pin7
        elif pin.number == 8:
            self._mode = PwmPinEnableMode.Pin7Pin8
        elif pin.number == 9:
            self._mode = PwmPinEnableMode.Pin7Pin8Pin9

        self.send_config()

    def stop_pin(self, pin: Pin):
        if pin.number == 8 and self._mode != PwmPinEnableMode.Pin7Pin8:
            self.logger.error(
                "You must disable PWM functionality on Pin 9 (PWM3) before disabling Pin 8's PWM functionality.")
        if pin.number == 7 and self._mode != PwmPinEnableMode.Pin7Pin8:
            self.logger.error(
                "You must disable PWM functionality on Pin 8 and 9 (PWM2 and PWM3) before disabling Pin 7's PWM functionality.")

        if pin.number == 7:
            self._mode = PwmPinEnableMode.NoPin
        elif pin.number == 8:
            self._mode = PwmPinEnableMode.Pin7
        elif pin.number == 9:
            self._mode = PwmPinEnableMode.Pin7Pin8

        self.send_config()

    def set_duty_cycle(self, pin: Pin, value: float):
        dc_value = round(value * 65535)
        reg = [dc_value & 0xff, dc_value >> 8]

        if pin.number == 7:
            self._duty_cycle_pin7 = reg
        elif pin.number == 8:
            self._duty_cycle_pin8 = reg
        elif pin.number == 9:
            self._duty_cycle_pin9 = reg

        self.send_config()

    def send_config(self):
        configuration = [DeviceCommands.PwmConfig, self._mode, self._frequency] \
                        + self._duty_cycle_pin7 \
                        + self._duty_cycle_pin8 \
                        + self._duty_cycle_pin9

        self._board._send_peripheral_config_packet(configuration)
