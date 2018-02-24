from typing import List

import usb.core
import usb.util
import threading
from treehopper.api.HardwarePwmManager import HardwarePwmManager
from treehopper.api.HardwareSpi import HardwareSpi
from treehopper.api.HardwareUart import HardwareUart
from treehopper.utils.EventHandler import EventHandler
from treehopper.api.Pin import Pin
from treehopper.api.HardwarePwm import HardwarePwm
from treehopper.api.HardwareI2c import HardwareI2c
from treehopper.api.DeviceCommands import DeviceCommands
from treehopper.api.SoftPwmManager import SoftPwmManager


class TreehopperUsb:
    """Core object for communicating with Treehopper USB boards"""

    def __init__(self, dev: usb.core.Device):
        self._pin_listener_thread = threading.Thread(target=self._pin_listener)
        self._dev = dev
        self._comms_lock = threading.Lock()

        self._pin_report_endpoint = 0x81
        self._peripheral_response_endpoint = 0x82
        self._pin_config_endpoint = 0x01
        self._peripheral_config_endpoint = 0x02

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

        self.uart = HardwareUart(self)
        self.i2c = HardwareI2c(self)
        self.spi = HardwareSpi(self)
        self.hardware_pwm_manager = HardwarePwmManager(self)
        self._soft_pwm_manager = SoftPwmManager(self)
        self.pwm1 = HardwarePwm(self.pins[7])
        self.pwm2 = HardwarePwm(self.pins[8])
        self.pwm3 = HardwarePwm(self.pins[9])

        self._led = False
        self._connected = False
        self._pin_report_received = EventHandler(self)

    def connect(self):
        """Connect to the board.

        Calling this method will connect to the board and start the pin listener update thread. Repeated calls
        to this method are ignored."""
        if self._connected:
            return

        self._dev.set_configuration()
        self._connected = True
        self._pin_listener_thread.start()

    def disconnect(self):
        """Disconnect from the board.

        This method disconnects from the board and stops the pin listener update thread. Repeated calls to this
        method are ignored."""
        if not self._connected:
            return

        self._connected = False
        self._pin_listener_thread.join()
        usb.util.dispose_resources(self._dev)

    def _pin_listener(self):
        while self._connected:
            try:
                data = self._dev.read(self._pin_report_endpoint, 41, 1000)
                i = 1
                if data[0] == 0x02:
                    for pin in self.pins:
                        pin._update_value(data[i], data[i+1])
                        i += 2
            except:
                pass
        return

    def _send_pin_config(self, data: List[int]):
        self._dev.write(self._pin_config_endpoint, data)

    def _send_peripheral_config_packet(self, data: List[int]):
        self._dev.write(self._peripheral_config_endpoint, data)

    def _receive_comms_response_packet(self, num_bytes_to_read: int) -> List[int]:
        return self._dev.read(self._peripheral_response_endpoint, num_bytes_to_read)

    @property
    def led(self) -> bool:
        """
        [Property] Gets or sets the state of the LED.
        :return: the state of the LED
        """
        return self._led

    @led.setter
    def led(self, val: bool):
        """
        [Property] Gets or sets the state of the LED.
        :param val: the new state of the LED
        """
        self._led = val
        data = [DeviceCommands.LedConfig, self._led]
        self._dev.write(self._peripheral_config_endpoint, data)

    @property
    def connected(self):
        """[Property] Gets whether the board is connected."""
        return self._connected
