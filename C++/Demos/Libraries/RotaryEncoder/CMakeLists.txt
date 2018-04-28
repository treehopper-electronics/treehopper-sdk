cmake_minimum_required(VERSION 3.6)
cmake_policy(SET CMP0015 NEW)
project(RotaryEncoder)

set(CMAKE_CXX_STANDARD 11)

include_directories(../../../API/inc)

set(SOURCE_FILES
        RotaryEncoder.cpp
        )

add_executable(RotaryEncoder ${SOURCE_FILES})

target_link_libraries(RotaryEncoder treehopper)