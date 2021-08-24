from time import sleep
from treehopper.api import *
from treehopper.libraries.sensors.optical import Tsl2591

board = find_boards()[0]
board.connect()
sensor = Tsl2591(board.i2c)

while board.connected:
    print(sensor.lux)
    sleep(0.1)
