from threading import Lock
from typing import TYPE_CHECKING, Dict

from treehopper.api import DeviceCommands

if TYPE_CHECKING:
    from treehopper.api.TreehopperUsb import TreehopperUsb, Pin

class SoftPwmPinConfig:
    def __init__(self):
        self.duty_cycle = 0
        self.pulse_width = 0
        self.use_pulse_width = True
        self.ticks = 0
        self.pin = []  # type: Pin

class SoftPwmManager:
    def __init__(self, board: "TreehopperUsb"):
        self._board = board
        self._pins: Dict[int, SoftPwmPinConfig] = dict()
        self._lock = Lock()
        self._resolution = 0.25

    def start_pin(self, pin: "Pin"):
        if pin in self._pins.keys():
            return

        config = SoftPwmPinConfig()
        config.pin = pin

        self._pins[pin.number] = config
        self.update_config()

    def stop_pin(self, pin: "Pin"):
        del self._pins[pin.number]
        self.update_config()

    def set_duty_cycle(self, pin: "Pin", duty: float):
        with self._lock:
            self._pins[pin.number].duty_cycle = duty
            self._pins[pin.number].use_pulse_width = False
            self.update_config()

    def set_pulse_width(self, pin: "Pin", pulse_width: float):
        with self._lock:
            self._pins[pin.number].pulse_width = pulse_width
            self._pins[pin.number].use_pulse_width = False
            self.update_config()

    def get_duty_cycle(self, pin: "Pin"):
        if not self._pins[pin.number]:
            return 0

        return self._pins[pin.number].duty_cycle

    def get_pulse_width(self, pin: "Pin"):
        if not self._pins[pin.number]:
            return 0

        return self._pins[pin.number].pulse_width

    def update_config(self):
        if len(self._pins) > 0:
            for config in self._pins.values():
                if config.use_pulse_width:
                    config.ticks = config.pulse_width / self._resolution
                    config.duty_cycle = config.ticks / 65535

                else:
                    config.ticks = config.duty_cycle * 65535
                    config.pulse_width = config.ticks * self._resolution

            ordered_values = list(self._pins.values())
            ordered_values.sort(key=lambda x: x.ticks)

            count = len(ordered_values) + 1
            config = [DeviceCommands.SoftPwmConfig, count]

            time = 0

            for j in range(count):
                if j < len(ordered_values):
                    ticks = ordered_values[j].ticks - time
                else:
                    ticks = 65535 - time

                tmr_val = int(65535 - ticks)
                if j == 0:
                    config.append(0)
                else:
                    config.append(ordered_values[j - 1].pin.number)

                config.append(tmr_val >> 8)
                config.append(tmr_val & 0xff)
                time += ticks

            self._board._send_peripheral_config_packet(config)

        else:
            self._board._send_peripheral_config_packet([DeviceCommands.SoftPwmConfig, 0])

