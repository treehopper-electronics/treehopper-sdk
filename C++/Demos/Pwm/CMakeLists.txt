cmake_minimum_required(VERSION 3.7)
project(Pwm)

set(CMAKE_CXX_STANDARD 11)

include_directories(../../API/Treehopper/inc)

set(SOURCE_FILES
        Pwm.cpp
        )

add_executable(Pwm ${SOURCE_FILES})

target_link_libraries(Pwm Treehopper)