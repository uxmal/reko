cmake_minimum_required ( VERSION 3.10 FATAL_ERROR )
list(INSERT CMAKE_MODULE_PATH 0 "${REKO_SRC}/../cmake")

include(msbuild2cmake)

message(STATUS "Building native libraries")
invoke_cmake(
	BUILD_DIR ${CMAKE_BINARY_DIR}/build/${REKO_PLATFORM}/${CMAKE_BUILD_TYPE}
	DIRECTORY ${CMAKE_SOURCE_DIR}
	REKO_COMPILER ${REKO_COMPILER}
	PASS_VARIABLES
		CMAKE_BUILD_TYPE
)