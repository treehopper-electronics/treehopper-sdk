cmake_minimum_required(VERSION 3.7)
project(Treehopper)

set(CMAKE_CXX_STANDARD 11)

set(PROJECT_SOURCE_DIR src/)

file(GLOB COMMON_SRC ${PROJECT_SOURCE_DIR}/*.cpp)

if(WIN32)
    set(TARGET_BUILD_PLATFORM win)
endif(WIN32)

if(UNIX)
    set(TARGET_BUILD_PLATFORM linux)
endif(UNIX)

if(APPLE)
    set(TARGET_BUILD_PLATFORM mac)
endif(APPLE)

file(GLOB PLATFORM_SRC ${PROJECT_SOURCE_DIR}/${TARGET_BUILD_PLATFORM}/*.cpp)
set(SOURCE_FILES ${COMMON_SRC} ${PLATFORM_SRC})

include_directories(inc)

add_library(Treehopper ${SOURCE_FILES})