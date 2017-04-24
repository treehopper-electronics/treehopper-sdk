cmake_minimum_required(VERSION 3.6)
project(I2cDeviceScanner)

set(CMAKE_CXX_STANDARD 11)

include_directories(../../API/Treehopper/inc)

set(SOURCE_FILES
        I2cDeviceScanner.cpp
        )

add_executable(I2cDeviceScanner ${SOURCE_FILES})

target_link_libraries(I2cDeviceScanner Treehopper)