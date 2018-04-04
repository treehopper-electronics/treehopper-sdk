from time import sleep
from treehopper.api import find_boards
from treehopper.libraries.sensors.temperature import Lm75

board = find_boards()[0]
board.connect()
sensor = Lm75(board.i2c, 1, 1, 1)
sensor.auto_update_when_property_read = False

while board.connected:
    sensor.update()
    print("Temperature: {} °C ({} °F)".format(sensor.celsius, sensor.fahrenheit))
    sleep(0.1)
