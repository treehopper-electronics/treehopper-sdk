from treehopper.api import *
from time import sleep

board = find_boards()[0]
board.connect()
board.pwm1.duty_cycle = 0.99
board.pwm1.enabled = True

while True:
    sleep(0.1)
