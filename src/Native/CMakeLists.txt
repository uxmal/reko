project(reko-addons)

cmake_minimum_required(VERSION 3.0)

if(CMAKE_BUILD_TYPE STREQUAL "")
	set(CMAKE_BUILD_TYPE Debug)
endif()

include (CheckSymbolExists)
configure_file(${CMAKE_SOURCE_DIR}/config.h.in ${CMAKE_BINARY_DIR}/config.h)

include_directories(
	${CMAKE_BINARY_DIR}
	${CMAKE_SOURCE_DIR}
)

if(UNIX OR MINGW)
	find_package(PkgConfig REQUIRED)
	pkg_check_modules(CAPSTONE REQUIRED capstone)
else()
	find_library(CAPSTONE_LIBRARIES capstone)
endif()

add_subdirectory(common)
add_subdirectory(ArmNative)