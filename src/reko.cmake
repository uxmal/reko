cmake_minimum_required(VERSION 3.10 FATAL_ERROR)
list(INSERT CMAKE_MODULE_PATH 0 "${REKO_SRC}/../cmake")

include(msbuild2cmake)

set(cmake_arguments "")
list(APPEND cmake_arguments
	-DCMAKE_BUILD_TYPE=${CMAKE_BUILD_TYPE}
	-DREKO_SRC=${REKO_SRC}
	-DREKO_PLATFORM=${REKO_PLATFORM}
	-DTARGET=${TARGET}
)

if("${TARGET}" STREQUAL "git_hash")
	list(APPEND cmake_arguments
		-DGIT_HASH_OUTPUT=${GIT_HASH_OUTPUT}
	)
endif()


set(BUILD_DIR ${CMAKE_BINARY_DIR}/build/${TARGET})

if(ACTION STREQUAL clean)
	clean_project(${BUILD_DIR})
else()
	invoke_cmake(
		BUILD_DIR ${BUILD_DIR}
		DIRECTORY ${REKO_SRC}/../
		TARGET ${TARGET}
		GENERATOR ${REKO_COMPILER}
		# variables needed by CMakeLists.txt, that must be forwarded
		EXTRA_ARGUMENTS ${cmake_arguments}	
	)
endif()