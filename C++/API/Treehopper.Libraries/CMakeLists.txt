cmake_minimum_required(VERSION 3.6)
project(TreehopperLibraries)

add_definitions(-DTREEHOPPERLIBRARIES_EXPORTS)
add_definitions(-DUNICODE)

set(CMAKE_CXX_STANDARD 11)

include_directories(./inc)
include_directories(../Treehopper/inc)

file(GLOB_RECURSE SOURCE_FILES src/*.cpp)

add_library(TreehopperLibraries SHARED ${SOURCE_FILES})
target_link_libraries(TreehopperLibraries Treehopper)

install(TARGETS TreehopperLibraries
    RUNTIME DESTINATION bin
    ARCHIVE DESTINATION lib
    LIBRARY DESTINATION lib)