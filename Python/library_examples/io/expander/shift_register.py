from time import sleep
from treehopper.api import *
from treehopper.libraries.io.expander import ChainableShiftRegisterOutput


board = find_boards()[0]
board.connect()
shift1 = ChainableShiftRegisterOutput(spi=board.spi, latch_pin=board.pins[9], speed_mhz=0.1)
shift2 = ChainableShiftRegisterOutput(parent=shift1)

shift1.auto_flush = False
shift2.auto_flush = False

while True:
    for i in range(255):
        shift1.current_value = [0, 0]
        shift2.current_value = [i, 0]
        shift1.flush()
        sleep(0.05)
