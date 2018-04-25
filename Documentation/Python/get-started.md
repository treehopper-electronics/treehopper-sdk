\page geting-started Getting Started

# Prerequisites
Treehopper requires Python 3.6 or later. This is not a particularly new version of Python (it has already been superseded by 3.7), but some regular Python users are still using Python 3.5.

While Linux and macOS often come with Python installed, these can sometimes be quite ancient. To check which version you have, invoke:
```
python3 -V
```

## Anaconda
If you don't already have a Python setup, we recommend installing the [Anaconda distribution](https://www.anaconda.com/download/).

On Windows and macOS, it's a standard installer you can click through to install. By the way, you can choose to install Anaconda during the Visual Studio 2017 installation process, so you may already have an installation.

On Linux, it's a text-mode shell script. To invoke it, give it executable permission before running it:
```
~ $ cd ~/Downloads
~/Downloads $ chmod a+x Anaconda*
~/Downloads $ ./Anaconda*
```

In Linux and Windows, you can optionally add Anaconda to your PATH when prompted. This will make it easier to execute, but will shadow a potentially existing Python distribution. On Mac, Anaconda is automatically added to your PATH.

This Getting Started guide will assume you're using Anaconda (with the `base` environment), but you can loosely follow along if you have another 3.6 distribution.

## Launching Python
We need a terminal prompt with Python and pip on its path. Assuming you're using Anaconda, we'll need to launch on Anaconda environment.

 - **In Windows:** You can invoke an Anaconda-endowed terminal by going to **Start > Anaconda3 > Anaconda Prompt**. Since we'll be installing packages, we'll need write access to the installation. If you installed Anaconda with administrative privileges in a non-user directory (like Program Files), you'll need to right-click on **Anaconda Prompt** and choose **Run as administrator...**.
 - **In macOS:** Anaconda should already be in your path, but if not, you can launch Terminal and type `source /anaconda3/bin/activate base`. If you prefer, you can also launch Anaconda Navigator, go to **Environments**, click on the arrow next to base (root), and choose **Open Terminal**.
 - **In Linux:** If Anaconda is in your path, simply open a terminal prompt. Otherwise, you can run `source anaconda3/bin/activate base` if Anaconda is not already in your path.

You can verify your installation in Windows:
```
> python -V
Python 3.6.2 :: Anaconda custom (64-bit)
```

or on macOS or Linux:
```
$ python3 -V
Python 3.6.4 :: Anaconda, Inc.
```

## Installing Treehopper packages
The Treehopper Python package is hosted on PyPI, so we'll use `pip` to install it:

```
(base) $ pip install treehopper
```

If you don't have write permission, this will fail with a `PermissionError`. On macOS or Linux, run the command with `sudo`. On Windows, make sure you started the Anaconda Prompt with **Run as administrator**.

## Linux: Adding udev rules
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

# Interactive LED
Let's start an interactive Python shell and manually connect to a board and turn on its LED.

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

All of Treehopper's capabilities can be used from an interactive Python shell like this, which makes the Python package an invaluable tool for diagnosing and debugging.

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