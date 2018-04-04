from time import sleep

from treehopper.api import *
from treehopper.libraries.sensors.inertial import Bno055

board = find_boards()[0]
board.connect()

imu = Bno055(board.i2c)
while board.connected:
    print(imu.quaternion)
    sleep(0.1)
