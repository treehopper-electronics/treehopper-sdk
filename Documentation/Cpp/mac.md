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

Finally, install the `treehopper` package using `vcpkg`:

    $ ./vcpkg install treehopper

## Manually using CMake

If you do not wish to use `vcpkg`, you can manually clone %Treehopper's repo, open the C++ folder in CMake, generate an XCode or Makefile-based project, and build it.

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