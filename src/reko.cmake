cmake_minimum_required ( VERSION 3.10 FATAL_ERROR )
list(INSERT CMAKE_MODULE_PATH 0 "${REKO_SRC}/../cmake")

include(msbuild2cmake)

invoke_cmake(
	BUILD_DIR ${CMAKE_BINARY_DIR}/build/${TARGET}
	DIRECTORY ${REKO_SRC}/../
	TARGET ${TARGET}
	GENERATOR ${REKO_COMPILER}
	PLATFORM ${REKO_PLATFORM}
	# variables needed by CMakeLists.txt, that must be forwarded
	EXTRA_ARGUMENTS
		-DCMAKE_BUILD_TYPE=${CMAKE_BUILD_TYPE}
		-DREKO_SRC=${REKO_SRC}
		-DREKO_PLATFORM=${REKO_PLATFORM}
		-DTARGET=${TARGET}
)