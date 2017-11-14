from treehopper.api import find_boards
from time import sleep

board = find_boards()[0]
board.connect()

while True:
    board.led = not board.led
    sleep(0.1)
