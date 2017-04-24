cmake_minimum_required(VERSION 3.6)
cmake_policy(SET CMP0015 NEW)
project(DigitalIn)

set(CMAKE_CXX_STANDARD 11)

include_directories(../../API/Treehopper/inc)

set(SOURCE_FILES
        DigitalIn.cpp
        )

add_executable(DigitalIn ${SOURCE_FILES})

target_link_libraries(DigitalIn Treehopper)