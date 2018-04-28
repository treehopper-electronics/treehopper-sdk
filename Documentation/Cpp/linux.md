\page linux Linux

Treehopper's C++ API has Linux support through direct calls into [libusb-1.0](http://libusb.info/), the ubiquitous USB library that's found on almost every Linux device out there.

# Prerequisites

You can use any C++ IDE or text editor

## CMake
%Treehopper's C++ API is built with CMake. Once built, you can use whatever build system you wish (Autotools, raw Makefiles, etc), but this tutorial will assume you're using CMake.

Use your package manager to install it; e.g.:

    sudo zypper install cmake

## udev rules

By default, most Linux-based operating systems restrict normal users from interacting with USB devices. To enable non-root users to interact with %Treehopper boards, you must add a udev rule.

Paste and run this quick snippet into a terminal window:

    $ sudo echo "SUBSYSTEM==\"usb\", ATTRS{idVendor}==\"10c4\", ATTRS{idProduct}==\"8a7e\", MODE:=\"666\", GROUP=\"users\"" > /etc/udev/rules.d/999-treehopper.rules

Verify the rule was created:

    $ cat /etc/udev/rules.d/999-treehopper.rules 
    SUBSYSTEM=="usb", ATTRS{idVendor}=="10c4", ATTRS{idProduct}=="8a7e", MODE:="666", GROUP="users"

Note that, for some distributions, `sudo` may not be configured to allow this. You can launch a root shell and repeat the command.

    $ sudo su
    # echo "SUBSYSTEM==\"usb\", ATTRS{idVendor}==\"10c4\", ATTRS{idProduct}==\"8a7e\", MODE:=\"666\", GROUP=\"users\"" > /etc/udev/rules.d/999-treehopper.rules

# Building the library

%Treehopper's C++ API is not distributed in binary form; you will need to build it before including and linking to your project.

## Using vcpkg

The recommended way of obtaining %Treehopper's C++ API is with Microsoft's [open-source vcpkg tool](https://github.com/Microsoft/vcpkg), a C/C++ library manager that will automatically download, build, and install packages to your system. Vcpkg contains more than 900 different commonly-used C/C++ packages.

### Installing Vcpkg

If you do not already have Vcpkg installed, clone or download the [vcpkg repo](https://github.com/Microsoft/vcpkg). Here, we'll assume you cloned to `C:\src\vcpkg`.

Then, build the `vcpkg` executable:

    $ ./bootstrap-vcpkg.sh

Finally, integrate vcpkg into Visual Studio:

    $ ./vcpkg integrate install

### Installing %Treehopper's C++ API

%Treehopper's C++ API requires `libusb-1.0` when running on Linux. While almost all Linux distributions have `libusb-1.0` installed, to build the %Treehopper API on Linux, you'll need the development headers and lib file.

In the future, Vcpkg will automatically grab this, however, the Vcpkg `libusb` package is currently broken on Linux, so instead, use your distribution's package manager to install a `libusb-1.0.0-dev` package:

    $ sudo apt-get install libusb-dev

\warning Make sure you install a `libusb-1.0` series package, and not an ancient `libusb0` library (which `libusb-dev` is).

Treehopper and Treehopper.Libraries are distributed in a single `treehopper` package that you can install:

    $ ./vcpkg install treehopper

## Manually using CMake

If you do not wish to use `vcpkg`, you can manually clone %Treehopper's repo, open the C++ folder in CMake, generate a project, and build it. This will result in an `.so` and `.a` file for use in your projects.

# Blinky

Create a new Visual C++ Windows Console application in Visual Studio 2017. Replace the contents of the main file with the following code:

    #include "Treehopper\ConnectionService.h"

    using namespace Treehopper;

    int main()
    {
        auto board = ConnectionService::instance().getFirstDevice();
        board.connect();

        for (int i = 0; i<20; i++)
        {
            board.led(!board.led());
            this_thread::sleep_for(chrono::milliseconds(100));
        }

        board.disconnect();
        return 0;
    }

This code will get a reference to the first board found connected to the system, connect to it, and blink the LED 20 times before disconnecting and exiting.
