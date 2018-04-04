from typing import List
from treehopper.libraries.flushable import Flushable
from treehopper.libraries.displays import Led, LedDriver
from treehopper.utils.utils import byte_to_bit_list
from typing import List
from treehopper.libraries.displays.character_display import CharacterDisplay
from treehopper.libraries.displays.led import Led
from treehopper.libraries.displays.led import LedDriver


class SevenSegmentDigit(Flushable):
    """A 7-segment digit"""
    def __init__(self, leds: List[Led]):
        self.leds = leds
        self.drivers = []  # type: list[LedDriver]
        for led in leds:
            led.driver.auto_flush = False

            if led.driver not in self.drivers:
                self.drivers.append(led.driver)

        self._char = " "
        self._decimal_point = False
        self.flush(True)

    def flush(self, force=False):
        for driver in self.drivers:
            driver.flush(force)

    char_table = [
        # 0x00  0x01  0x02  0x03  0x04  0x05  0x06  0x07  0x08  0x09  0x0A  0x0B  0x0C  0x0D  0x0E  0x0F
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  # 0x00
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  # 0x10
        0x00, 0x82, 0x21, 0x00, 0x00, 0x00, 0x00, 0x02, 0x39, 0x0F, 0x00, 0x00, 0x00, 0x40, 0x80, 0x00,  # 0x20
        0x3F, 0x06, 0x5B, 0x4F, 0x66, 0x6D, 0x7D, 0x07, 0x7f, 0x6f, 0x00, 0x00, 0x00, 0x48, 0x00, 0x53,  # 0x30
        0x00, 0x77, 0x7C, 0x39, 0x5E, 0x79, 0x71, 0x6F, 0x76, 0x06, 0x1E, 0x00, 0x38, 0x00, 0x54, 0x3F,  # 0x40
        0x73, 0x67, 0x50, 0x6D, 0x78, 0x3E, 0x00, 0x00, 0x00, 0x6E, 0x00, 0x39, 0x00, 0x0F, 0x00, 0x08,  # 0x50
        0x63, 0x5F, 0x7C, 0x58, 0x5E, 0x7B, 0x71, 0x6F, 0x74, 0x02, 0x1E, 0x00, 0x06, 0x00, 0x54, 0x5C,  # 0x60
        0x73, 0x67, 0x50, 0x6D, 0x78, 0x1C, 0x00, 0x00, 0x00, 0x6E, 0x00, 0x39, 0x30, 0x0F, 0x00, 0x00  # 0x70
    ]

    @property
    def char(self):
        return self._char

    @char.setter
    def char(self, value):
        if self._char == value:
            return

        self._char = value
        leds = byte_to_bit_list(SevenSegmentDigit.char_table[ord(self._char)])
        for i in range(8):
            self.leds[i].state = leds[i]

        if self.auto_flush:
            self.flush()


    @property
    def decimal_point(self):
        return self._decimal_point

    @decimal_point.setter
    def decimal_point(self, value):
        if self._decimal_point == value:
            return

        self._decimal_point = value
        self.leds[7].state = self._decimal_point


class SevenSegmentDisplay(CharacterDisplay):
    """7-segment display"""
    def __init__(self, leds: List[Led], right_to_left=False):
        if len(leds) % 8 != 0:
            raise ValueError("Leds should contain a multiple of 8 segments")

        self.num_digits = len(leds)//8

        super().__init__(self.num_digits, 1)

        self.digits = []  # type: List[SevenSegmentDigit]
        self.drivers = [] # type: List[LedDriver]

        for i in range(self.num_digits):
            digit = SevenSegmentDigit(leds[i*8:i*8+8])
            self.digits.append(digit)

        if not right_to_left:
            self.digits.reverse()

        for digit in self.digits:
            digit.auto_flush = False

            for driver in digit.drivers:
                if driver not in self.drivers:
                    self.drivers.append(driver)

    def _write_char(self, character):
        if character == ".":
            self.cursor_left -= 1
            self.digits[self.cursor_left].decimal_point = True
        else:
            self.digits[self.cursor_left].char = character
        pass

    def _clear(self):
        for digit in self.digits:
            digit.char = " "
        pass

    def _update_cursor(self):
        pass

    def flush(self, force=False):
        for driver in self.drivers:
            driver.flush(force)
