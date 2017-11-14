import usb.core
import usb.util
from treehopper.api.DeviceCommands import DeviceCommands


class TreehopperUsb:
    """Core object for communicating with Treehopper USB boards"""

    def __init__(self, dev: usb.core.Device):
        self.dev = dev
        self.pinReportEndpoint = 0x81
        self.peripheralResponseEndpoint = 0x82
        self.pinConfigEndpoint = 0x01
        self.peripheralConfigEndpoint = 0x02
        self._led = False

    def connect(self):
        self.dev.set_configuration()

    def disconnect(self):
        usb.util.dispose_resources(self.dev)

    def _set_led(self, val):
        self._led = val
        data = [DeviceCommands.LedConfig, self._led]
        self.dev.write(self.peripheralConfigEndpoint, data)

    def _get_led(self) -> bool:
        return self._led

    led = property(_get_led, _set_led)