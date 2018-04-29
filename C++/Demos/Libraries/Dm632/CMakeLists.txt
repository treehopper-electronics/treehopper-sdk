cmake_minimum_required(VERSION 3.6)
cmake_policy(SET CMP0015 NEW)
project(Dm632)

set(CMAKE_CXX_STANDARD 11)

include_directories(../../../API/inc)

set(SOURCE_FILES
        Dm632.cpp
        )

add_executable(Dm632 ${SOURCE_FILES})

target_link_libraries(Dm632 treehopper)