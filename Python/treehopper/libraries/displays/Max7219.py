from treehopper.libraries import SpiDevice
from treehopper.api.Spi import ChipSelectMode, SpiBurstMode
from treehopper.libraries.displays.LedDriver import LedDriver
from treehopper.libraries.displays.Led import Led
import logging


class Opcode:
    Noop = 0
    Digit0 = 1
    Digit1 = 2
    Digit2 = 3
    Digit3 = 4
    Digit4 = 5
    Digit5 = 6
    Digit6 = 7
    Digit7 = 8
    DecodeMode = 9
    Intensity = 10
    ScanLimit = 11
    Shutdown = 12
    DisplayTest = 15


class Max7219(LedDriver):
    _num_devices = 0

    def __init__(self, spi_module=None, load_pin=None, parent=None, speed_mhz=1):
        super().__init__(64, True, False)
        self._logger = logging.getLogger(__name__)

        if speed_mhz > 10.0:
            self._logger.error("The MAX7219 supports a maximum clock rate of 10 MHz.")

        if (spi_module == None or load_pin == None) and parent == None:
            self._logger.error("You must specify a parent MAX7219, or an SPI module and load pin")

        if spi_module is not None:
            self.dev = SpiDevice(spi_module=spi_module, chip_select=load_pin, chip_select_mode=ChipSelectMode.PulseHighAtEnd, speed_mhz=speed_mhz)
            self.address = 0
        else:
            self.dev = parent.dev
            self.address = parent.address + 1

        ## Gets or sets whether the driver should auto-flush when an LED is modified
        self.auto_flush = True
        self._shutdown = True
        self._scan_limit = 0
        self.state = [0] * 8
        self.send_test(False)

        ## Gets or sets the maximum digit index (from 0 to 7) that should be scanned
        self.scan_limit = 7
        self._send(Opcode.DecodeMode, 0)
        self.clear()

        ## Gets or sets whether the display should be disabled
        self.shutdown = False
        self._set_global_brightness(1)

        ## Whether to use the native LED order of the MAX7219, or the standard for Treehopper drivers.
        ## If True, LED segments are ordered G-A, DP. If False, LED segments are ordered A-DP.
        ## All Treehopper LED driver libraries are, by default, standardized
        ## so that the LSB of the driver corresponds with segment "A",
        ## the 7th bit corresponds with "G" and the MSB corresponds to the "DP" segment.
        ## This allows easy consumption of display libraries that require ordered collections
        ## of segments.
        ## However, you may choose to work with this library using the native LED ordering,
        ## where the LSB corresponds with segment "G", the 7th bit corresponds to segment "A"
        ## and the MSB corresponds to segment "DP".
        self.use_native_led_order = False


    def _send(self, opcode, data):
        if Max7219._num_devices < self.address + 1:
            Max7219._num_devices = self.address + 1

        offset = self.address * 2
        max_bytes = Max7219._num_devices * 2

        spi_data = [0] * max_bytes
        spi_data[offset + 1] = opcode
        spi_data[offset] = data

        spi_data.reverse()

        self.dev.send_receive(spi_data, SpiBurstMode.BurstTx)

    @property
    def shutdown(self):
        """[Property] Gets or sets whether the display should be disabled"""
        return self._shutdown

    @shutdown.setter
    def shutdown(self, value):
        """[Property] Gets or sets whether the display should be disabled"""
        if self._shutdown == value:
            return

        self._shutdown = value
        self._send(Opcode.Shutdown, not self._shutdown)

    @property
    def scan_limit(self):
        """[Property] Gets or sets the maximum digit index (from 0 to 7) that should be scanned"""
        return self._scan_limit

    @scan_limit.setter
    def scan_limit(self, value):
        """[Property] Gets or sets the maximum digit index (from 0 to 7) that should be scanned"""
        if value == self._scan_limit:
            return
        self._scan_limit = value
        self._send(Opcode.ScanLimit, self._scan_limit)

    def send_decode_mode(self, decode_mode):
        self._send(Opcode.DecodeMode, decode_mode)

    def send_test(self, test):
        self._send(Opcode.DisplayTest, test)

    def _set_global_brightness(self, value: float):
        self._send(Opcode.Intensity, round(value * 15.0))

    def flush(self, force=False):
        if self.auto_flush and not force:
            return

        for i in range(8):
            self._send(i + 1, self.state[i])

    def _led_state_changed(self, led: Led):
        digit = led.channel // 8
        segment = led.channel % 8

        if not self.use_native_led_order:
            if segment < 7:
                segment = 6 - segment

        if led.state:
            self.state[digit] |= 1 << segment
        else:
            self.state[digit] &= ~(1 << segment)

        if self.auto_flush:
            self._send(digit + 1, self.state[digit])

    def _led_brightness_changed(self, led: Led):
        pass
