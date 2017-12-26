from typing import List

from treehopper.libraries.displays.CharacterDisplay import CharacterDisplay
from treehopper.libraries.displays.Led import Led


class SevenSegmentDisplay(CharacterDisplay):
    def __init__(self, leds: List[Led], right_to_left=False):
        if len(leds) % 8 != 0:
            raise ValueError("Leds should contain a multiple of 8 segments")
        super().__init__(int(len(leds)/8), 1)
