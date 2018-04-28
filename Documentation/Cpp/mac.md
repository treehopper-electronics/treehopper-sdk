\page mac macOS

# Prerequisites

%Treehopper supports both LLVM and GCC compilers on macOS.

# Building the library

%Treehopper's C++ API is not distributed in binary form; you will need to build it before including and linking to your project.

## Using vcpkg

The recommended way of obtaining %Treehopper's C++ API is with Microsoft's [open-source vcpkg tool](https://github.com/Microsoft/vcpkg), a C/C++ library manager that will automatically download, build, and install packages to your system. Vcpkg contains more than 900 different commonly-used C/C++ packages.

### Installing Vcpkg

If you do not already have Vcpkg installed, clone or download the [vcpkg repo](https://github.com/Microsoft/vcpkg). Here, we'll assume you cloned to `C:\src\vcpkg`.

While %Treehopper supports LLVM, Vcpkg requires C++ Filesystem support, which currently requires GCC 6. Install it using homebrew:

    brew install gcc

Then, build the `vcpkg` executable:

    $ ./bootstrap-vcpkg.sh

Finally, integrate vcpkg into Visual Studio:

    $ ./vcpkg integrate install

### Installing %Treehopper's C++ API

Treehopper and Treehopper.Libraries are distributed in a single `treehopper` package that you can install:

    $ ./vcpkg install treehopper

## Manually using CMake

If you do not wish to use `vcpkg`, you can manually clone %Treehopper's repo, open the C++ folder in CMake, generate an XCode or Makefile-based project, and build it. This will result in a `.DyLib` and `.a` file for use in your projects.

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
