cmake_minimum_required(VERSION 3.7)
cmake_policy(SET CMP0015 NEW)
project(Blink)

set(CMAKE_CXX_STANDARD 11)

include_directories(../../API)

set(SOURCE_FILES
        Blink.cpp
        stdafx.cpp
        stdafx.h
        targetver.h)

add_executable(Blink ${SOURCE_FILES})

target_link_libraries(Blink Treehopper)