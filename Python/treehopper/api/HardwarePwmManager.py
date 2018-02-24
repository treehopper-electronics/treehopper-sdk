from treehopper.api.Pin import Pin
from treehopper.api.DeviceCommands import DeviceCommands
import logging


class PwmPinEnableMode:
    NoPin, Pin7, Pin7Pin8, Pin7Pin8Pin9 = range(4)


class HardwarePwmFrequency:
    Freq_732Hz, Freq_183Hz, Freq_61Hz = range(3)


class HardwarePwmManager:
    def __init__(self, board):
        self._board = board
        self._duty_cycle_pin7 = [0, 0]
        self._duty_cycle_pin8 = [0, 0]
        self._duty_cycle_pin9 = [0, 0]
        self._frequency = HardwarePwmFrequency.Freq_732Hz
        self._mode = PwmPinEnableMode.NoPin
        self.logger = logging.getLogger(__name__)

    @property
    def microseconds_per_tick(self) -> float:
        return 1000000.0 / (self.frequency_hz * 65536)

    @property
    def period_microseconds(self) -> float:
        return 1000000 / self.frequency_hz

    @property
    def frequency_hz(self) -> int:
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
