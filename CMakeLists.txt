cmake_minimum_required(VERSION 3.10 FATAL_ERROR)
list(INSERT CMAKE_MODULE_PATH 0 "${CMAKE_SOURCE_DIR}/cmake")
find_package(Dotnet 2.0 REQUIRED)

if(WIN32)
	find_package(WIX)
endif()

project(Reko NONE)
enable_testing()

# set main framework
set(REKO_LIB_FRAMEWORK "net8.0")
set(REKO_EXE_FRAMEWORK "net8.0")
set(REKO_WINFORMS_EXE_FRAMEWORK "net8.0-windows")

if(WIN32)
	set(REKO_COMPILER "Visual Studio 17 2022")
endif()

add_subdirectory(src)
