\page linux Linux

%Treehopper's C++ API has Linux support

# Prerequisites

This section will discuss requirements necessary to build %Treehopper's C++ API on Linux, along with build and runtime requirements for apps that use %Treehopper.

## Toolchains

You'll obviously need a C++ toolchain to build C++ apps. Every Linux distribution has a package manager that will allow you to easily install a toolchain. Consult the documentation for your particular distribution.

%Treehopper is written in standards-compliant C++11. On Linux, the only compiler we regularly test is `gcc` (though `llvm` is supported on macOS, and should work on Linux).

## libusb

%Treehopper's C++ API uses [libusb-1.0](http://libusb.info/), the ubiquitous USB library that's found in almost every device running Linux.
 
To build the the %Treehopper API, you'll need the development headers for libusb 1.0. Most Linux distributions have dev packages that contain headers needed to compile and link to installed libraries; e.g.

     $ sudo apt-get install libusb-1.0.0-dev

\warning %Treehopper uses libusb-1.0, not the ancient libusb-0.1 package; In most distributions, the `libusb-dev` package is actually the libusb-0.1 development package, not the libusb-1.0 one. Be sure to install libusb-1.0-dev.

## Rumtime requirements & permissions (udev rules)

Unless you statically link to libusb, the only runtime dependency is `libusb-1.0.so` (or equivalent) present on the system. Even simple Linux devices like consumer WiFi routers usually have this library present, so this shouldn't be an issue for most users.

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

## CMake

%Treehopper's C++ API is built with CMake. Once the API is built, you can use whatever build system you wish (Autotools, raw Makefiles, etc) with your own applications, but this tutorial will assume you're using CMake.

Use your package manager to install it; e.g.:

    sudo zypper install cmake

## Using vcpkg

The recommended way of obtaining %Treehopper's C++ API is with Microsoft's new [open-source vcpkg tool](https://github.com/Microsoft/vcpkg), a cross-platform C/C++ library manager that will automatically download, build, and install packages to your system. Vcpkg contains more than 900 different commonly-used C/C++ packages.

### Installing Vcpkg

If you do not already have Vcpkg installed, clone or download the [vcpkg repo](https://github.com/Microsoft/vcpkg).

Then, build the `vcpkg` executable:

    $ ./bootstrap-vcpkg.sh

### Installing %Treehopper's C++ API

Once Vcpkg is installed, install the `treehopper` package:

    $ ./vcpkg install treehopper

## Manually using CMake

If you do not wish to use `vcpkg`, you can manually clone %Treehopper's repo, open the C++ folder in CMake, generate a project, and build/install it.

# Blinky

Once you have the treehopper package installed, you're ready to build your first project.

Create a `blinky` directory for your new project. Inside, create a `main.c` file:

```{.c}
// main.c
#include <Treehopper/ConnectionService.h>
#include <chrono>

using namespace Treehopper;
using namespace std::chrono;

int main()
{
    auto board = ConnectionService::instance().getFirstDevice();
    board.connect();

    for (int i = 0; i<20; i++)
    {
        board.led(!board.led());
        this_thread::sleep_for(milliseconds(100));
    }

    board.disconnect();
    return 0;
}
```

Also, create a `CMakeLists.txt` file inside the directory:

    # CMakeLists.txt
    cmake_minimum_required(VERSION 3.0)

    set(CMAKE_CXX_STANDARD 11)

    project(blinky)

    find_package(treehopper REQUIRED)

    add_executable(main main.cpp)
    target_link_libraries(main treehopper)

Now, run `cmake`. If you're using Vcpkg, make sure to append the toolchain file:

    $ cmake .. "-DCMAKE_TOOLCHAIN_FILE=/path/to/vcpkg/scripts/buildsystems/vcpkg.cmake"

This will create a `Makefile` in the project directory. You can simply

    $ make

Or, CMake can build it for you, too:

    $ cmake --build .

In this case, CMake will refresh the `CMakeLists.txt` file in case you have committed updates.

Now that your program has been built, you can execute it:

    $ ./main

This code will get a reference to the first board found connected to the system, connect to it, and blink the LED 20 times before disconnecting and exiting.
