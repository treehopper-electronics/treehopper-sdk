\page geting-started Getting Started

The [Treehopper pip package](https://pypi.org/project/treehopper/) requires Python 3.6 or later, pyusb (which will auto-install when you install the treehopper package), and libusb.

# Windows
On Windows, we'll need to install a Python distribution, libusb, and finally the Treehopper Python package.

## Python
Windows does not ship with a Python distribution, but instead will prompt you to install it from the Microsoft Store when you first attempt to invoke it. This option is suitable for most basic users of Python.

For more serious Python users, we recommend installing the Anaconda distribution of Python, which includes a more advanced package manager and the ability to create multiple Python environments to avoid version conflicts. More information is available on the [Anaconda home page](https://www.anaconda.com/).

## libusb
You'll need the libusb DLL installed on your system to be able to use Treehopper in Python. To install, visit the [libusb web site](https://libusb.info/) to download the pre-built Windows binaries. You will also need [7-Zip](https://www.7-zip.org/download.html) to extract the download archive. This download includes the DLLs (compiled both with MSVC and MinGW), along with libs and header files should you want to write a C/C++ application that uses libusb. 

For our purposes, we only need the DLL. We'll use the MSVC-compiled version found in MS64/dll/libusb-1.0.dll. To avoid having to copy this file into each new project separately, you can drag it into C:\Windows\system32, where it will be available system-wide. Note: there might be other software on your computer that has already installed libusb-1.0.dll to this location. There's no need to overwrite this file in this case.

## Treehopper package
Next, install the latest version of Treehopper's Python library by launching PowerShell or Command Prompt and typing
```
PS> pip install treehopper
```

# Linux
On Linux-based systems, in addition to installing the Treehopper Python package with pip, we'll need to make sure you have a recent-enough Python distribution and a properly-configured udev rules list.

## Python
While Linux distributions often come with Python installed, these installations can be quite ancient. To check which version you have, invoke:
```
python3 -V
```

Treehopper requires Python 3.6 or newer. If your installation is too old, try updating it with your package manager. Consult your distribution's documentation for help on doing this.

For more serious Python users, we recommend installing the Anaconda distribution of Python, which includes a more advanced package manager and the ability to create multiple Python environments to avoid version conflicts. More information is available on the [Anaconda home page](https://www.anaconda.com/).

# Adding udev rules
By default, most Linux-based operating systems restrict normal users from interacting with USB devices. To enable non-root users to interact with %Treehopper boards, you must add a udev rule.

Paste and run this quick snippet into a terminal window:
```
$ sudo echo "SUBSYSTEM==\"usb\", ATTRS{idVendor}==\"10c4\", ATTRS{idProduct}==\"8a7e\", MODE:=\"666\", GROUP=\"users\"" > /etc/udev/rules.d/999-treehopper.rules
```
Verify the rule was created:
```
$ cat /etc/udev/rules.d/999-treehopper.rules 

SUBSYSTEM=="usb", ATTRS{idVendor}=="10c4", ATTRS{idProduct}=="8a7e", MODE:="666", GROUP="users"
```

Note that, for some distributions, `sudo` may not be configured to allow this. You can launch a root shell and repeat the command.
```
$ sudo su
# echo "SUBSYSTEM==\"usb\", ATTRS{idVendor}==\"10c4\", ATTRS{idProduct}==\"8a7e\", MODE:=\"666\", GROUP=\"users\"" > /etc/udev/rules.d/999-treehopper.rules
```

## Treehopper package
Next, install the latest version of Treehopper's Python library by launching a terminal window and typing
```
$ pip3 install treehopper
```

## libusb
It is unlikely that you do not have libusb-1.0 installed, but if you receive errors when running the example code below, check your package manager to ensure it is installed.

# Mac
On Mac, you'll need at least Python 3.6 installed, along with libusb and the Treehopper Python package.

## Python
macOS already includes an ancient Python 2.7 distribution. Python has an official [macOS installer available on their web site](https://www.python.org/downloads/mac-osx/), which will install in `/usr/local/bin`.

For more serious Python users, we recommend installing the Anaconda distribution of Python, which includes a more advanced package manager and the ability to create multiple Python environments to avoid version conflicts. More information is available on the [Anaconda home page](https://www.anaconda.com/).

## libusb
We recommend using the [Homebrew package manager](https://brew.sh/) to install libusb:

```
$ brew install libusb
```

## Treehopper Python package
Next, install the latest version of Treehopper's Python library by launching a terminal window and typing
```
$ pip3 install treehopper
```
\warning Note the `pip3` command, which will invoke the Python 3-series version of the package utility, instead of the original Python 2.7-based `pip` found on the system.


# Interactive LED
Now that we have our environment set up, let's start an interactive Python shell and manually connect to a board and turn on its LED.

Launch `python` on Windows, or `python3` on macOS or Linux (`python` will also alias to `python3` if you're in an Anaconda prompt).

Type (or copy and paste) each of these commands, one by one (you don't need to include the comments, of course):

```
>>> from treehopper.api import find_boards  # import the find_boards() function
>>> board = find_boards()[0]                # get a list of boards and select the first one
>>> board.connect()                         # connect to the board
>>> board.led = True                        # turn on the LED
>>> board.led = False                       # turn off the LED
>>> board.disconnect()                      # disconnect from the board
>>> quit()                                  # quit the interactive shell
$
```

# Blinky script
Open a text editor and paste these contents in:
```
from treehopper.api import *
from time import sleep

board = find_boards()[0]
board.connect()
while board.connected:
    board.led = not board.led
    sleep(0.1)
```

Save the file as **blinky.py** in your home directory, return to your command prompt, and execute it:

```
$ python blinky.py
```

Notice that the script exits immediately once you unplug the board. While Treehopper's Python API doesn't support hotplug detection, it *does* support hotplug removal notification via the board.connected property.
