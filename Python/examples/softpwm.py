from treehopper.api import *
from time import sleep
import threading

board = find_boards()[0]
board.connect()
board.pins[19].mode = PinMode.SoftPwm


def waiter():
    input("Press [ENTER] to stop and disconnect")
    board.disconnect()


threading.Thread(target=waiter).start()

while board.connected:
    while board.pins[19].duty_cycle > 0.01:
        board.pins[19].duty_cycle -= 0.01
        sleep(0.01)
    while board.pins[19].duty_cycle < 0.99:
        board.pins[19].duty_cycle += 0.01
        sleep(0.01)
