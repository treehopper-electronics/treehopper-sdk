from typing import List

import usb.core
import usb.util
import threading
from events import Events
from treehopper.api.DeviceCommands import DeviceCommands
from treehopper.api.HardwarePwmManager import HardwarePwmManager
from treehopper.utils.EventHandler import EventHandler
from treehopper.api.Pin import Pin
from treehopper.api.HardwarePwm import HardwarePwm

class TreehopperUsb:
    """Core object for communicating with Treehopper USB boards"""

    def __init__(self, dev: usb.core.Device):
        self.pin_listener_thread = threading.Thread(target=self._pin_listener)
        self._dev = dev

        self.pin_report_endpoint = 0x81
        self.peripheral_response_endpoint = 0x82
        self.pin_config_endpoint = 0x01
        self.peripheral_config_endpoint = 0x02

        self.pins = []  # type: List[Pin]
        for i in range(20):
            self.pins.append(Pin(self, i))

        self.pins[0].name = "Pin 0 (SCK)"
        self.pins[1].name = "Pin 1 (MISO)"
        self.pins[2].name = "Pin 2 (MOSI)"
        self.pins[3].name = "Pin 3 (SDA)"
        self.pins[4].name = "Pin 4 (SCL)"
        self.pins[5].name = "Pin 5 (TX)"
        self.pins[6].name = "Pin 6 (RX)"
        self.pins[7].name = "Pin 7 (PWM1)"
        self.pins[8].name = "Pin 8 (PWM2)"
        self.pins[9].name = "Pin 9 (PWM3)"

        self.hardware_pwm_manager = HardwarePwmManager(self)
        self.pwm1 = HardwarePwm(self.pins[7])
        self.pwm2 = HardwarePwm(self.pins[8])
        self.pwm3 = HardwarePwm(self.pins[9])

        self._led = False
        self._connected = False
        self.pin_report_received = EventHandler(self)

    def connect(self):
        self._dev.set_configuration()
        self._connected = True
        self.pin_listener_thread.start()

    def disconnect(self):
        self._connected = False
        self.pin_listener_thread.join()
        usb.util.dispose_resources(self._dev)

    def _pin_listener(self):
        while self._connected:
            try:
                data = self._dev.read(self.pin_report_endpoint, 41, 1000)
                i = 1
                if data[0] == 0x02:
                    for pin in self.pins:
                        pin._update_value(data[i], data[i+1])
                        i += 2
            except:
                pass
        return

    def _send_pin_config(self, data):
        self._dev.write(self.pin_config_endpoint, data)

    def _send_peripheral_config_packet(self, data):
        self._dev.write(self.peripheral_config_endpoint, data)

    @property
    def led(self) -> bool:
        return self._led

    @led.setter
    def led(self, val):
        self._led = val
        data = [DeviceCommands.LedConfig, self._led]
        self._dev.write(self.peripheral_config_endpoint, data)

    @property
    def connected(self):
        return self._connected
