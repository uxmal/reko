#
# To make it easier to call CMake from MSBuild, this special CMake script is used.
# CMake can be ran in "generation" mode (where CMakeLists.txt is used) or in "program" mode (-P flag), where a script file is ran
# but no solution or makefile will be generated
#
# By using this feature, we can have MSBuild call a transitional script called "reko.cmake"
# "reko.cmake" will receive variables that we explicitly passed from MSBuild, create a build directory,
# then re-invoke CMake again but in "generation" mode.
#
# Before the final invocation, we can run checks against the passed MSBuild variables, adapt them to better suit the CMake model,
# then forward them to the new CMake's invocation which will use this file, "CMakeLists.txt"
#

#
# Global variables:
# `CMAKE_BUILD_TYPE`, which is forwarded to CMakeLists.txt
# `REKO_PLATFORM`, which is expected to be passed from $(Platform)
# `TARGET`, which is the name of the target (defined in CMakeLists.txt) to build
#

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

#
# This function runs CMake in "generation" mode,
# similarly to what would be done manually when building a CMake project,
# this is done in 2 major steps: configuration and building
# 
# CONFIGURATION:
# - creating a build directory for the CMake generated files (specified by `BUILD_DIR`)
# - invoking CMake against the directory holding CMakeLists.txt (specified by `DIRECTORY`)
# - optionally specify a generator to use (specified by `GENERATOR`), e.g. "Unix Makefiles"
# - specify any variable to be passed to CMakeLists.txt
# This last part can be controlled by 2 different options: `PASS_VARIABLES` and `EXTRA_ARGUMENTS`:
# - `PASS_VARIABLES` specifies the name of existing CMake variables that you wish to forward from this script to CMakeLists.txt
#    (e.g. CMAKE_BUILD_TYPE)
# - `EXTRA_ARGUMENTS` can be used to specify any extra argument to be passed to CMake.
#    we can pass new variables in the form -DFOO=BAR
# BUILDING:
# After the project has been configured, it can be built with "make", "msbuild", etc...
# CMake offers a facility to abstract it away by using "cmake --build <directory>"
# we can additionally pass a target name to build just that specific target instead of the whole solution.
# This is controlled by the `TARGET` parameter
#
function(invoke_cmake)
	cmake_parse_arguments(proj "QUICK_CONFIGURE" "DIRECTORY;BUILD_DIR;GENERATOR;TARGET;PLATFORM" "PASS_VARIABLES;EXTRA_ARGUMENTS" ${ARGN})
	
	## Beginning of the "configuration" phase

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

	# cmake <directory>
	set(CMAKE_ARGS ${proj_DIRECTORY})
	
	# append -G <generator> if specified or previously set
	if(proj_GENERATOR)
		list(APPEND CMAKE_ARGS "-G" "${proj_GENERATOR}")
	endif()

	if(WIN32)
		if(proj_PLATFORM STREQUAL "x64")
			 list(APPEND CMAKE_ARGS "-A" "x64")
		elseif(proj_PLATFORM STREQUAL "x86")
			list(APPEND CMAKE_ARGS "-A" "Win32")
		endif()
	endif()

	# This flag skips CMake compiler sanity checks ("Checking if C compiler works"),
	# which typically involve compiling small dummy programs
	# it can be used to speedup the initial generation phase at the expense of safety
	if(proj_QUICK_CONFIGURE)
		message(STATUS "Skipping compiler sanity checks")
		list(APPEND CMAKE_ARGS
			"-DCMAKE_C_COMPILER_FORCED=ON"
			"-DCMAKE_CXX_COMPILER_FORCED=ON"
		)		
	endif()

	# pass-in (forward) variables
	foreach(var_name ${proj_PASS_VARIABLES})
		set(var_value "${${var_name}}")
		list(APPEND CMAKE_ARGS "-D${var_name}=${var_value}")
	endforeach()

	# add extra arguments
	if(proj_EXTRA_ARGUMENTS)
		set(CMAKE_ARGS "${CMAKE_ARGS};${proj_EXTRA_ARGUMENTS}")
	endif()

	#message(STATUS "CMake args: ${CMAKE_ARGS}")

	execute_process(
		COMMAND ${CMAKE_COMMAND} ${CMAKE_ARGS}
		WORKING_DIRECTORY ${proj_BUILD_DIR}
		RESULT_VARIABLE retcode
	)

	if(NOT ${retcode} EQUAL 0)
		message(FATAL_ERROR "[CMake] configuration failed for: ${proj_DIRECTORY}")
	endif()

	## Beginning of the "build" phase

	set(build_args "")

	# set the target to build, if provided
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

function(process_project name path)
	set(BUILD_DIR ${path}/build/${REKO_PLATFORM}/${CMAKE_BUILD_TYPE})

	if(ACTION STREQUAL "clean")
		clean_project(${name} ${path} ${BUILD_DIR})
	else()
		invoke_cmake(${name} ${path} ${BUILD_DIR})
	endif()
endfunction()

message("== Configuration ==")
message("=> Build Type: ${CMAKE_BUILD_TYPE}")
message("=> Generator : ${REKO_COMPILER}")
message("=> Platform  : ${REKO_PLATFORM}")
message("=> Target    : ${TARGET}")
message("")

set(REKO_SRC ${CMAKE_CURRENT_LIST_DIR}/../src)

# AnyCPU is not a valid native platform, so we filter it out here
set(REKO_ACTUAL_PLATFORM "")
if(NOT REKO_PLATFORM STREQUAL "AnyCPU")
	set(REKO_ACTUAL_PLATFORM ${REKO_PLATFORM})
endif()
