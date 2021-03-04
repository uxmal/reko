cmake_minimum_required ( VERSION 3.10 FATAL_ERROR )
list(INSERT CMAKE_MODULE_PATH 0 "${CMAKE_SOURCE_DIR}/cmake")
find_package(Dotnet 2.0 REQUIRED)

if(WIN32)
	find_package(WIX)
endif()

project(Reko NONE)
enable_testing()

set(REKO_LIB_FRAMEWORK "netstandard2.1")
set(REKO_EXE_FRAMEWORK "net5.0")

add_subdirectory(src)
