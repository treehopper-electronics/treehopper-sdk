from time import sleep
from treehopper.api import *
from treehopper.libraries.sensors.optical import Vcnl4010

board = find_boards()[0]
board.connect()
sensor = Vcnl4010(i2c=board.i2c)
sensor.auto_update_when_property_read = False

while board.connected:
    sensor.update()
    print(sensor.meters)
    print(sensor.lux)
    sleep(0.1)
