from time import sleep
from treehopper.api import *
from treehopper.libraries.sensors.optical import Isl29125
from colr import color


def data_received(sender, red, green, blue):
    max_val = max(red, max(green, blue))
    r = 255 * red / max_val
    g = 255 * green / max_val
    b = 255 * blue / max_val
    print(color(f'     red={red:4.3f}, green={green:4.3f}, blue={blue:4.3f}     ', fore=(0, 0, 0), back=(r, g, b)))


board = find_boards()[0]
board.connect()
sensor = Isl29125(board.i2c, board.pins[15])
sensor.interrupt_received += data_received

while True:
    sleep(1000)
