from time import sleep

from treehopper.api import *
from treehopper.libraries.displays import Max7219, SevenSegmentDisplay

board = find_boards()[0]
board.connect()

# how many MAX7219s do you have?
num_drivers = 3

parent = Max7219(spi_module=board.spi, load_pin=board.pins[15])
leds = parent.leds
for i in range(num_drivers-1):
    parent = Max7219(parent=parent)
    leds = parent.leds + leds

# quick check of all the LEDs
for led in leds:
    led.state = True
    sleep(0.01)

for led in leds:
    led.state = False
    sleep(0.01)

display = SevenSegmentDisplay(leds)
i=0
while True:
    display.clear()
    display.write(i)
    i += 1
