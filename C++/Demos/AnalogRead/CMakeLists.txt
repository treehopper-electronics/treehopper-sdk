cmake_minimum_required(VERSION 3.6)
project(AnalogRead)

set(CMAKE_CXX_STANDARD 11)

include_directories(../../API/inc)

set(SOURCE_FILES
        AnalogRead.cpp
        )

add_executable(AnalogRead ${SOURCE_FILES})

target_link_libraries(AnalogRead treehopper)