#!/usr/bin/env python

"""ADC Demo

This demo initializes pin 0 as an analog input and illustrates using the
analog_voltage_changed event to notify user code (in this case, a lambda
expression) when the voltage at the pin exceeds its threshold.

This script will run until terminated or board.disconnect() is called by
pressing [ENTER].
"""

from treehopper.api import find_boards, PinMode

board = find_boards()[0]
board.connect()
pin = board.pins[0]
pin.mode = PinMode.AnalogInput
pin.analog_voltage_changed += lambda sender, value: print(value)

input("Press [ENTER] to stop and disconnect\n")
board.disconnect()
