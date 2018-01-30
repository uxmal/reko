project(reko-addons)

cmake_minimum_required(VERSION 3.0)

if(CMAKE_BUILD_TYPE STREQUAL "")
	set(CMAKE_BUILD_TYPE Debug)
endif()

add_subdirectory(ArmNative)