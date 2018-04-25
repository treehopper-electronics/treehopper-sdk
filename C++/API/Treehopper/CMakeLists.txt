cmake_minimum_required(VERSION 3.6)
project(Treehopper)

add_definitions(-DTREEHOPPER_EXPORTS)
add_definitions(-DUNICODE)

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

add_library(Treehopper SHARED ${SOURCE_FILES})

if(APPLE)
    find_library(CORE_FOUNDATION CoreFoundation)
    find_library(IOKIT IOKit)
    target_link_libraries(Treehopper pthread ${CORE_FOUNDATION} ${IOKIT})
elseif(UNIX)
    target_link_libraries(Treehopper usb-1.0 pthread)
elseif(WIN32)
    target_link_libraries(Treehopper winusb setupapi)
endif(APPLE)

install(TARGETS Treehopper
    RUNTIME DESTINATION bin
    ARCHIVE DESTINATION lib
    LIBRARY DESTINATION lib)