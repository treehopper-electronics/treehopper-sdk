from treehopper.api import *
from treehopper.libraries.sensors.inertial import Adxl345

board = find_boards()[0]
board.connect()
imu = Adxl345(alt_address=True, i2c=board.i2c)
while True:
    print(imu.accelerometer)
