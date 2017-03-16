# Core Functionality
Treehopper's core functionality consists of ConnectionService board discovery, name and serial number customization, and, of course, board peripheral access.

## Board Discovery (ConnectionService)
Most Treehopper language APIs provide a static instance of the ConnectionService class that can be used to discover attached Treehopper boards. Depending on the language you're using, this class might provide events that you can subscribe to so you can receive notifications when boards are plugged and unplugged. Treehopper's core API is designed from the ground up to support an arbitrarily large number of connected devices, so most language APIs provide a dynamic list of attached boards. Some language APIs allow you to query the ConnectionService instance to discover boards that match certain criteria (i.e., have a specific serial number or name).

## Simultaneous Access
Only one connection can be made to a Treehopper board, and the library supports connecting to as many Treehopper boards as you want. This has some important repercussions:
 - <b>Avoid creating instances of ConnectionService; use the static instance property it provides for all access</b>. If you want to share a board between different areas of your code (for example, between decoupled ViewModels in a MVVM-style application), you must share the board object (or the ConnectionService instance that can be used to retrieve the board object). Do not create instances of ConnectionService in each module and attempt to access the boards concurrently; this will fail.
 - <b>When possible, ConnectionService will query the OS --- not the device directly --- about its name and serial number</b>. This allows an applications to scan all the boards attached to a computer; even if the boards are connected to other applications.

## Name and Serial Number
Each Treehopper board has a serial number and a name. Both of these properties can be set by the user. Note that these properties correspond to the ProductName and SerialNumber that are part of the USB specification, which means they'll be visible across your operating system. Note that Windows does not refresh the name of the device in Device Manager.

## Peripherals
Treehopper's main functionality --- its MCU peripherals --- are accessed 

### Digital & Analog Pins
Treehopper boards have up to 20 \subpage pins — each of which can be used as analog inputs, digital inputs, or digital outputs.

### I<sup>2</sup>C
Treehopper supports an SMBus-compliant \subpage i2c master.

### SPI
Treehopper supports a three-wire \subpage spi master.