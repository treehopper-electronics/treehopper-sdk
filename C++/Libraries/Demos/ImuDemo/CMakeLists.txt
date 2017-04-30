cmake_minimum_required(VERSION 3.6)
cmake_policy(SET CMP0015 NEW)
project(ImuDemo)

set(CMAKE_CXX_STANDARD 11)

include_directories(../../../API/Treehopper/inc)
include_directories(../../../API/Treehopper.Libraries/inc)

set(SOURCE_FILES
        ImuDemo.cpp
        )

add_executable(ImuDemo ${SOURCE_FILES})

target_link_libraries(ImuDemo Treehopper TreehopperLibraries)