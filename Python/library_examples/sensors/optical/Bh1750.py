from time import sleep

from treehopper.api import *
from treehopper.libraries.sensors.optical import Bh1750

board = find_boards()[0]
board.connect()
sensor = Bh1750(board.i2c, False)
while True:
    print(sensor.lux)
    sleep(0.1)
