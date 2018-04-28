\page windows Windows

# Prerequisites

The only currently-supported toolchain on Windows is the MSVC-based one found in [Visual Studio 2017](https://www.visualstudio.com/downloads/). 

Cygwin and MinGW-based build environments are not supported, so while you can use IDEs such as [JetBrain's CLion](https://www.jetbrains.com/clion/) for development, you must build your projects using the MSVC compiler.

# Building the library

%Treehopper's C++ API is not distributed in binary form; you will need to build it before including and linking to your project.

## Using vcpkg

The recommended way of obtaining %Treehopper's C++ API is with Microsoft's [open-source vcpkg tool](https://github.com/Microsoft/vcpkg), a C/C++ library manager that will automatically download, build, and install packages to your system. Vcpkg contains more than 900 different commonly-used C/C++ packages.

### Installing Vcpkg

If you do not already have Vcpkg installed, clone or download the [vcpkg repo](https://github.com/Microsoft/vcpkg). Here, we'll assume you cloned to `C:\src\vcpkg`.

Then, build the `vcpkg` executable:

    C:\src\vcpkg> .\bootstrap-vcpkg.bat

Finally, integrate vcpkg into Visual Studio:

    C:\src\vcpkg> .\vcpkg integrate install

### Installing %Treehopper's C++ API

Treehopper and Treehopper.Libraries are distributed in a single `treehopper` package that you can install:

    C:\src\vcpkg> .\vcpkg install treehopper

By default, this will build an x86 library. If you want to target x64, execute

    C:\src\vcpkg> .\vcpkg install treehopper:x64-windows

## Manually using CMake

If you do not wish to use `vcpkg`, you can manually clone %Treehopper's repo, open the C++ folder from Visual Studio 2017, and build the project collection. This will result in a `.dll` and `.lib` file for use in your projects.

# Blinky

Create a new Visual C++ Windows Console application in Visual Studio 2017. Replace the contents of the main file with the following code:

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

This code will get a reference to the first board found connected to the system, connect to it, and blink the LED 20 times before disconnecting and exiting.
