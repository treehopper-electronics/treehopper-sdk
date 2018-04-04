from time import sleep
from treehopper.api import *
from treehopper.libraries.displays import Ht16k33

board = find_boards()[0]
board.connect()
driver = Ht16k33(i2c=board.i2c, package=Ht16k33.Package.sop20)

while board.connected:
    for led in driver.leds:
        led.state = True
        sleep(0.04)

    for led in driver.leds:
        led.state = False
        sleep(0.04)
