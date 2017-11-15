from typing import List

import usb.core
import usb.util
import threading
from events import Events
from treehopper.api.DeviceCommands import DeviceCommands
from treehopper.utils.EventHandler import EventHandler
from treehopper.api.Pin import Pin

class TreehopperUsb:
    """Core object for communicating with Treehopper USB boards"""

    def __init__(self, dev: usb.core.Device):
        self.pin_listener_thread = threading.Thread(target=self._pin_listener)
        self._dev = dev
        self.pins = []  # type: List[Pin]
        for i in range(20):
            self.pins.append(Pin(self, i))

        self.pinReportEndpoint = 0x81
        self.peripheralResponseEndpoint = 0x82
        self.pinConfigEndpoint = 0x01
        self.peripheralConfigEndpoint = 0x02
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
                data = self._dev.read(self.pinReportEndpoint, 41, 1000)
                i = 1
                if data[0] == 0x02:
                    for pin in self.pins:
                        pin._update_value(data[i], data[i+1])
                        i += 2
            except:
                pass
        return

    def _send_pin_config(self, data):
        self._dev.write(self.pinConfigEndpoint, data)

    @property
    def led(self) -> bool:
        return self._led

    @led.setter
    def led(self, val):
        self._led = val
        data = [DeviceCommands.LedConfig, self._led]
        self._dev.write(self.peripheralConfigEndpoint, data)

    @property
    def connected(self):
        return self._connected
