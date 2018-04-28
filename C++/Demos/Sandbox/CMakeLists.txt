cmake_minimum_required(VERSION 3.6)
project(Sandbox)

set(CMAKE_CXX_STANDARD 11)

include_directories(../../API/inc)

set(SOURCE_FILES
        Sandbox.cpp
        )

add_executable(Sandbox ${SOURCE_FILES})

target_link_libraries(Sandbox treehopper)