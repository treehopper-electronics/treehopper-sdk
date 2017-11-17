from treehopper.api import *


def handler(sender, value):
    print(pin)


board = find_boards()[0]
board.connect()
pin = board.pins[0]
pin.mode = PinMode.DigitalInput
pin.digital_value_changed += handler

# this program will run until terminated or board.disconnect() is called
