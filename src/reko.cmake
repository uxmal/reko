cmake_minimum_required ( VERSION 3.10 FATAL_ERROR )

function(check_msys output)
	execute_process(
		COMMAND uname -o
		OUTPUT_VARIABLE ostype
		RESULT_VARIABLE result
	)
	if(NOT result EQUAL 0)
		set(${output} FALSE PARENT_SCOPE)
		return()
	endif()

	string(TOLOWER "${ostype}" ostype)
	string(STRIP "${ostype}" ostype)

	if(${ostype} STREQUAL "msys")
		set(${output} TRUE PARENT_SCOPE)
		return()
	endif()

	set(${output} FALSE PARENT_SCOPE)
endfunction()

function(clean_project name path build_dir)
	if(EXISTS ${build_dir})
		message(STATUS "Removing ${build_dir}")
		file(REMOVE_RECURSE ${build_dir})
	endif()
endfunction()

function(invoke_cmake)
	cmake_parse_arguments(proj "QUICK_CONFIGURE" "DIRECTORY;BUILD_DIR;GENERATOR;TARGET" "PASS_VARIABLES;VARIABLES" ${ARGN})
	
	if(NOT EXISTS ${proj_BUILD_DIR})
		file(MAKE_DIRECTORY ${proj_BUILD_DIR})
	endif()

	# set default generator for win32
	if(NOT proj_GENERATOR AND WIN32)
		if(NOT DEFINED IS_MSYS)
			check_msys(IS_MSYS)
		endif()
		message(STATUS "IS_MSYS: ${IS_MSYS}")
		if(IS_MSYS)
			set(proj_GENERATOR "MSYS Makefiles")
		elseif(MINGW)
			set(proj_GENERATOR "MinGW Makefiles")
		endif()
	endif()

	set(CMAKE_ARGS ${proj_DIRECTORY})
	if(proj_GENERATOR)
		list(APPEND CMAKE_ARGS -G ${proj_GENERATOR})
	endif()

	if(proj_QUICK_CONFIGURE)
		message(STATUS "Skipping compiler sanity checks")
		list(APPEND CMAKE_ARGS
			"-DCMAKE_C_COMPILER_FORCED=ON"
			"-DCMAKE_CXX_COMPILER_FORCED=ON"
		)		
	endif()

	foreach(var_name ${proj_PASS_VARIABLES})
		set(var_value "${${var_name}}")
		list(APPEND CMAKE_ARGS "-D${var_name}=${var_value}")
	endforeach()

	if(proj_VARIABLES)
		set(CMAKE_ARGS "${CMAKE_ARGS};${proj_VARIABLES}")
	endif()

	execute_process(
		COMMAND ${CMAKE_COMMAND} ${CMAKE_ARGS}
		WORKING_DIRECTORY ${proj_BUILD_DIR}
		RESULT_VARIABLE retcode
	)

	if(NOT ${retcode} EQUAL 0)
		message(FATAL_ERROR "[CMake] configuration failed for: ${proj_DIRECTORY}")
	endif()

	set(build_args "")

	if(proj_TARGET)
		list(APPEND build_args --target ${proj_TARGET})
	endif()

#	include(ProcessorCount)
#	ProcessorCount(NUM_THREADS)
#
#	if(IS_MSYS OR MINGW OR UNIX)
#		list(APPEND build_args -- -j ${NUM_THREADS})
#	endif()

	message(STATUS "Build Args: ${build_args}")

	execute_process(
		COMMAND ${CMAKE_COMMAND} --build . ${build_args}
		WORKING_DIRECTORY ${proj_BUILD_DIR}
		RESULT_VARIABLE retcode
	)

	if(NOT ${retcode} EQUAL 0)
		message(FATAL_ERROR "[CMake] build failed: ${proj_DIRECTORY}")
	endif()
endfunction()

message("== Configuration ==")
message("=> Build Type: ${CMAKE_BUILD_TYPE}")
message("=> Generator : ${REKO_COMPILER}")
message("=> Target    : ${TARGET}")
message("")

set(REKO_SRC ${CMAKE_CURRENT_LIST_DIR}/../)

set(REKO_LIB_FRAMEWORK "netstandard2.1")
set(REKO_EXE_FRAMEWORK "netcoreapp3.1")

invoke_cmake(
	QUICK_CONFIGURE
	BUILD_DIR ${CMAKE_BINARY_DIR}/build
	DIRECTORY ${REKO_SRC}
	TARGET ${TARGET}
	PASS_VARIABLES
		CMAKE_BUILD_TYPE
		REKO_PLATFORM
)