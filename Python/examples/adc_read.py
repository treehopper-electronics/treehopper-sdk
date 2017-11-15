from treehopper.api import find_boards, PinMode
from time import sleep

board = find_boards()[0]
board.connect()
pin = board.pins[0]
pin.mode = PinMode.AnalogInput

while True:
    print(pin.analog_voltage)
    sleep(0.1)
