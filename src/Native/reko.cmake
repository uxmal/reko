cmake_minimum_required(VERSION 3.10 FATAL_ERROR)
list(INSERT CMAKE_MODULE_PATH 0 "${REKO_SRC}/../cmake")

include(msbuild2cmake)

set(BUILD_DIR ${CMAKE_BINARY_DIR}/build/${REKO_PLATFORM}/${CMAKE_BUILD_TYPE})

if(ACTION STREQUAL "clean")
	clean_project(${BUILD_DIR})
else()
	message(STATUS "Building native libraries")
	invoke_cmake(
		BUILD_DIR ${BUILD_DIR}
		DIRECTORY ${CMAKE_SOURCE_DIR}
		GENERATOR ${REKO_COMPILER}
		EXTRA_ARGUMENTS
			-DCMAKE_BUILD_TYPE=${CMAKE_BUILD_TYPE}
			-DREKO_PLATFORM=${REKO_PLATFORM}
	)
endif()
