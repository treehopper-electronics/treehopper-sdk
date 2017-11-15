from treehopper.api import find_boards, PinMode
from time import sleep

board = find_boards()[0]
board.connect()
pin = board.pins[0]
pin.mode = PinMode.PushPullOutput

while True:
    pin.digital_value = not pin.digital_value  # toggle pin 0
    board.led = not board.led                  # also toggle the LED
    sleep(0.1)
