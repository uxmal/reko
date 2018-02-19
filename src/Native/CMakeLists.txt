project(reko-native)

cmake_minimum_required(VERSION 3.0)

set(CMAKE_CXX_STANDARD 11)
set(CMAKE_C_STANDARD 11)

# No library prefix for consistent naming (Windows has no lib prefix)
set(CMAKE_SHARED_LIBRARY_PREFIX "")

if(CMAKE_BUILD_TYPE STREQUAL "")
	set(CMAKE_BUILD_TYPE Debug)
endif()

if(NOT REKO_PATH)
	get_filename_component(
		REKO_PATH "../../"
		REALPATH
		BASE_DIR "${CMAKE_SOURCE_DIR}"
	)
	message(STATUS "Reko directory: ${REKO_PATH}")
endif()

set(IS_AMD64 FALSE)

if(CMAKE_SIZEOF_VOID_P EQUAL 8)
    set(IS_AMD64 TRUE)
endif()

include (CheckSymbolExists)
configure_file(${CMAKE_SOURCE_DIR}/config.h.in ${CMAKE_BINARY_DIR}/config.h)

include_directories(
	${CMAKE_BINARY_DIR}
	${CMAKE_SOURCE_DIR}
)

set(REKO_BUNDLED_LIBDIRS "")

if(IS_AMD64)
	list(APPEND REKO_BUNDLED_LIBDIRS "${REKO_PATH}/external/Capstone/X64")
else()
	list(APPEND REKO_BUNDLED_LIBDIRS "${REKO_PATH}/external/Capstone/X86")
endif()

if((UNIX OR MINGW) AND NOT MSVC)
	find_package(PkgConfig REQUIRED)
	pkg_check_modules(CAPSTONE REQUIRED capstone)
else()
	find_library(CAPSTONE_LIBRARIES capstone PATHS ${REKO_BUNDLED_LIBDIRS})
endif()

add_subdirectory(common)
add_subdirectory(ArmNative)
