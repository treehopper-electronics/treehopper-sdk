from treehopper.api import *

board = find_boards()[0]
board.connect()
board.i2c.enabled = True

for address in range(0, 128):
    try:
        print("Searching address {0} (0x{0:02x})... ".format(address), end="")
        board.i2c.send_receive(address, None, 0)
        print("Found device!")
    except RuntimeError:
        print("")

board.disconnect()  # closes the reader thread, allowing the script to exit
