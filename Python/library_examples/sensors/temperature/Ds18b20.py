from time import sleep

from treehopper.api import *
from treehopper.libraries.sensors.temperature import Ds18b20

board = find_boards()[0]
board.connect()
sensor = Ds18b20(board.uart)
sensor.auto_update_when_property_read = False
while True:
    sensor.update()
    print("Temperature: {} °C ({} °F)".format(sensor.celsius, sensor.fahrenheit))
    sleep(0.1)
