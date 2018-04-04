from time import sleep

from treehopper.api import *
from treehopper.libraries.sensors.inertial import Mpu9250

imu = None
board = find_boards()[0]
board.connect()
devices = Mpu9250.probe(board.i2c)
if len(devices) > 0:
    imu = devices[0]
else:
    raise RuntimeError("No MPU9250 or MPU6050 was found. Are you sure you're not using an MPU6050?")

imu.auto_update_when_property_read = False

while board.connected:
    imu.update()
    print(imu.accelerometer)
    print(imu.gyroscope)
    print(imu.magnetometer)
    print(imu.fahrenheit)
    sleep(0.5)
