from treehopper.api import *

board = find_boards()[0]
board.connect()
pin = board.pins[0]
pin.mode = PinMode.AnalogInput
pin.analog_voltage_changed += lambda sender, value: print(value)

input("Press [ENTER] to stop and disconnect\n")
board.disconnect()

# this program will run until terminated or board.disconnect() is called
