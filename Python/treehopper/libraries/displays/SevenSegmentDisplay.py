from typing import List

from treehopper.libraries.displays.SevenSegmentDigit import SevenSegmentDigit
from treehopper.libraries.displays.CharacterDisplay import CharacterDisplay
from treehopper.libraries.displays.Led import Led
from treehopper.libraries.displays.LedDriver import LedDriver


class SevenSegmentDisplay(CharacterDisplay):
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
