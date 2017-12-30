from treehopper.api import *


def handler(sender, value):
    print(pin)


board = find_boards()[0]
board.connect()
pin = board.pins[0]
pin.mode = PinMode.DigitalInput
pin.digital_value_changed += handler


input("Press [ENTER] to stop and disconnect\n")
board.disconnect()