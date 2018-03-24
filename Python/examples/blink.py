from treehopper.api import *
from time import sleep

board = find_boards()[0]
board.connect()
while board.connected:
    board.led = not board.led                  # also toggle the LED
    sleep(0.1)
