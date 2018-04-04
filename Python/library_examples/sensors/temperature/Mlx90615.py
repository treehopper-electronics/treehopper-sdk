from time import sleep

from treehopper.api import find_boards
from treehopper.libraries.sensors.temperature import Mlx90615

board = find_boards()[0]
board.connect()
sensor = Mlx90615(board.i2c)
sensor.object.auto_update_when_property_read = False
while board.connected:
    sensor.object.update()
    print("Temperature: {} °C ({} °F)".format(sensor.object.celsius, sensor.object.fahrenheit))
    sleep(0.1)
