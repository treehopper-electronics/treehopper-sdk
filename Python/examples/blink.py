from treehopper.api import find_boards
from time import sleep

board = find_boards()[0]

while True:
    board.led = not board.led
    sleep(0.1)