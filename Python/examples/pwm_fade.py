from treehopper.api import *
from time import sleep
import threading

board = find_boards()[0]
board.connect()
board.pwm1.enabled = True


def waiter():
    input("Press [ENTER] to stop and disconnect")
    board.disconnect()


threading.Thread(target=waiter).start()

while board.connected:
    while board.pwm1.duty_cycle > 0.01:
        board.pwm1.duty_cycle -= 0.01
        sleep(0.01)
    while board.pwm1.duty_cycle < 0.99:
        board.pwm1.duty_cycle += 0.01
        sleep(0.01)
