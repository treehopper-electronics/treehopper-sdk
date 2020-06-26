\mainpage Welcome

This documentation contains all Python-specific information for interfacing with Treehopper. For hardware documentation, or for documentation for other languages, visit [https://docs.treehopper.io/](https://docs.treehopper.io/).

Visit the [Getting Started](@ref geting-started) page to get all the software prerequisites installed and write your first Treehopper script.

## Features
Treehopper's Python API is designed for brevity and simplicity, making it well-suited for small scripts and interactive python sessions. The API supports Windows, macOS, and Linux, and runs under Python 3.6 or newer.

## Libraries
In addition to the main API that allows you to manipulate and sample pins on the Treehopper, the Python API also includes an ever-growing library full of drivers for many different peripheral ICs, including IMUs and other sensors; GPIO expanders, DACs and ADCs; LED drivers, character and graphical displays; and motor drivers, rotary encoders, and other motion devices.

## Modules
Treehopper's Python API is split across the following packages:
- treehopper.api: the base library. Provides GPIO, PWM, I2C, SPI, and base interface support. Requires one of these connectors:
- treehopper.libraries: provides support for more than 100 commonly-used ICs and peripherals.