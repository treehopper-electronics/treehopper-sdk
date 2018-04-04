from treehopper.api import I2C
from treehopper.utils.utils import bit_list_to_bytes
from treehopper.libraries import SMBusDevice
from treehopper.libraries.displays.led import LedDriver, Led


class Ht16k33(LedDriver):
    """Holtek HT16K33 LED driver

    """
    _command_brightness = 0xe0
    _command_blink = 0x80

    def __init__(self, i2c: I2C, package: int, a0=False, a1=False, a2=False, address=None):
        if not address:
            address = 0x70
            if a0:
                address |= 0x01
            if a1:
                address |= 0x02
            if a2:
                address |= 0x04

        LedDriver.__init__(self, num_leds=package, has_global_brightness_control=True,
                           has_individual_brightness_control=False)
        self._dev = SMBusDevice(address=address, i2c_module=i2c)
        self._dev.write_byte(0x21)
        self._package = package
        self._data = [False] * 128
        self._set_global_brightness(1.0)

    def _set_global_brightness(self, value: float):
        if self.brightness > 0.0:
            self._dev.write_byte(Ht16k33._command_brightness | round(self.brightness * 16) - 1)
            self._dev.write_byte(Ht16k33._command_blink | 0x01)
        else:
            self._dev.write_byte(Ht16k33._command_blink | 0x00)

    def flush(self, force=False):
        pass

    def _led_state_changed(self, led: Led):
        index = int(16 * int(led.channel / (self._package/8)) + (led.channel % (self._package/8)))
        self._data[index] = led.state
        if self.auto_flush:
            address = int(index / 8)
            self._dev.write_byte_data(address, bit_list_to_bytes(self._data)[address])

    def _led_brightness_changed(self, led: Led):
        pass


    class Package:
        sop20 = 64
        sop24 = 96
        sop28 = 128
