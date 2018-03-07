from time import sleep
from treehopper.api import find_boards
from treehopper.libraries.sensors.temperature import Mcp9808

board = find_boards()[0]
board.connect()
sensor = Mcp9808.probe(board.i2c)[0]
sensor.auto_update_when_property_read = False

while True:
    sensor.update()
    print("Temperature: {} °C ({} °F)".format(sensor.celsius, sensor.fahrenheit))
    sleep(0.1)
