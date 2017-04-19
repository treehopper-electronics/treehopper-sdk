cmake_minimum_required(VERSION 3.7)
project(treehopper VERSION 0.1 LANGUAGES CXX)

include(GNUInstallDirs)

add_subdirectory(API/Treehopper)
add_subdirectory(API/Treehopper.Libraries)

add_subdirectory(Demos/AnalogRead)
add_subdirectory(Demos/Blink)
add_subdirectory(Demos/DigitalIn)
add_subdirectory(Demos/I2cDeviceScanner)
add_subdirectory(Demos/Pwm)