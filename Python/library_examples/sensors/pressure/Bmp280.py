from time import sleep

from treehopper.api import *
from treehopper.libraries.sensors.pressure import Bmp280, Bme280

sensor = None
board = find_boards()[0]
board.connect()
devices = Bme280.probe(board.i2c)
if len(devices) > 0:
    sensor = devices[0]
else:
    raise RuntimeError("No BMP280 was found.")

sensor.auto_update_when_property_read = False

while board.connected:
    sensor.update()
    print(f"Pressure: {sensor.atm} Atm")
    print(f"Temperature: {sensor.celsius} °C ({sensor.fahrenheit} °F)")
    print(f"Altitude: {sensor.altitude} m")
    print(f"Humidity: {sensor.relative_humidity}%")
    sleep(0.5)
