from time import sleep

from treehopper.api import *
from treehopper.libraries.sensors.optical import Bh1750

board = find_boards()[0]
board.connect()
sensor = Bh1750(board.i2c, False)
sensor.auto_update_when_property_read = False
while True:
    sensor.update()
    print(sensor.lux)
    sleep(0.1)
